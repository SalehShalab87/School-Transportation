using Domain.Common;
using Domain.Products;
using Domain.Products.Events;
using FluentAssertions;

namespace Domain.UnitTests.Products;

public sealed class ProductTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_Should_Create_Valid_Aggregate_And_Raise_Domain_Event()
    {
        var product = Product.Create("Router", "School bus router", 99m, 5, Now);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Router");
        product.CreatedAtUtc.Should().Be(Now);
        product.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProductCreatedDomainEvent>();
    }

    [Theory]
    [InlineData("", "Description", 10, 1)]
    [InlineData("Name", "", 10, 1)]
    [InlineData("Name", "Description", 0, 1)]
    [InlineData("Name", "Description", 10, -1)]
    public void Create_Should_Protect_Domain_Invariants(
        string name,
        string description,
        decimal price,
        int stock)
    {
        var act = () => Product.Create(name, description, price, stock, Now);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void SoftDelete_Should_Be_Idempotent_And_Raise_One_Event()
    {
        var product = Product.Create("Router", "Description", 10m, 1, Now);
        product.ClearDomainEvents();

        product.SoftDelete(Now.AddMinutes(1));
        product.SoftDelete(Now.AddMinutes(2));

        product.IsDeleted.Should().BeTrue();
        product.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProductSoftDeletedDomainEvent>();
    }
}
