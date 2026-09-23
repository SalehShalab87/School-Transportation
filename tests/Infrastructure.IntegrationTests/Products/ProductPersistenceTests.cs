using Application;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Domain.Products;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Messaging.Outbox;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Products;

public sealed class ProductPersistenceTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task UnitOfWork_Should_Persist_Product_And_Outbox_In_Same_Save()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = fixture.Container.GetConnectionString()
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        await using var provider = services.BuildServiceProvider();

        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();

            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();

            repository.Add(Product.Create("Router", "Description", 10m, 1, clock.UtcNow));
            await unitOfWork.SaveChangesAsync();
        }

        await using (var verifyScope = provider.CreateAsyncScope())
        {
            var db = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            (await db.Products.CountAsync()).Should().Be(1);
            (await db.Set<OutboxMessage>().CountAsync()).Should().Be(1);
        }
    }
}
