namespace Domain.Products;

/// <summary>
/// Command-side aggregate repository. It tracks aggregates but never commits.
/// The application command owns the transaction through IUnitOfWork.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Product product);
}
