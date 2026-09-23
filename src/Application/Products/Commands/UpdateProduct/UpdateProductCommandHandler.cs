using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Application.Products.DTOs;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result<ProductDto>.Failure(Error.NotFound("Products.NotFound", "Product was not found."));

        product.Update(
            request.Product.Name,
            request.Product.Description,
            request.Product.Price,
            request.Product.Stock,
            clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ProductDto>.Success(ProductDto.FromEntity(product));
    }
}
