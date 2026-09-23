using Application.Products.DTOs;

namespace Application.Products.Abstractions;

/// <summary>
/// Query-side abstraction. Reads can use optimized projections without loading aggregates.
/// </summary>
public interface IProductReadService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
