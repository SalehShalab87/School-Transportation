using Application.Common;
using Application.Products.Abstractions;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductReadService readService)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await readService.GetByIdAsync(request.ProductId, cancellationToken);
        return product is null
            ? Result<ProductDto>.Failure(Error.NotFound("Products.NotFound", "Product was not found."))
            : Result<ProductDto>.Success(product);
    }
}
