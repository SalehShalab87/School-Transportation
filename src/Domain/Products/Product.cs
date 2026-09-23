using Domain.Common;
using Domain.Products.Events;

namespace Domain.Products;

/// <summary>
/// Sample aggregate demonstrating rich domain behavior. Replace Products with
/// real business modules (Students, Fleet, Planning, Trips, etc.) in the target system.
/// </summary>
public sealed class Product : AggregateRoot, ISoftDeletable
{
    private Product()
    {
        // Required by EF Core.
    }

    private Product(
        Guid id,
        string name,
        string description,
        decimal price,
        int stock,
        DateTimeOffset createdAtUtc) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Product Create(
        string name,
        string description,
        decimal price,
        int stock,
        DateTimeOffset now)
    {
        Validate(name, description, price, stock);

        var product = new Product(
            Guid.CreateVersion7(),
            name.Trim(),
            description.Trim(),
            price,
            stock,
            now);

        product.RaiseDomainEvent(
            new ProductCreatedDomainEvent(product.Id, product.Name, now));

        return product;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        int stock,
        DateTimeOffset now)
    {
        if (IsDeleted)
            throw new DomainException("A deleted product cannot be updated.");

        Validate(name, description, price, stock);

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        Stock = stock;
        UpdatedAtUtc = now;
    }

    public void SoftDelete(DateTimeOffset now)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        UpdatedAtUtc = now;
        RaiseDomainEvent(new ProductSoftDeletedDomainEvent(Id, now));
    }

    private static void Validate(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        if (name.Trim().Length > 200)
            throw new DomainException("Product name must not exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description is required.");

        if (description.Trim().Length > 1000)
            throw new DomainException("Product description must not exceed 1000 characters.");

        if (price <= 0)
            throw new DomainException("Product price must be greater than zero.");

        if (stock < 0)
            throw new DomainException("Product stock cannot be negative.");
    }
}
