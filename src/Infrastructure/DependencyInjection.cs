using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Application.Common.Messaging;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Outbox;
using Infrastructure.Persistence;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsql.UseNetTopologySuite();
                }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
