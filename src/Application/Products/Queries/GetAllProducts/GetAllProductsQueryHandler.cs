using Application.Common;
using Application.Products.Abstractions;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(IProductReadService readService)
    : IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await readService.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<ProductDto>>.Success(products);
    }
}
