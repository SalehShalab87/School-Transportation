using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Application.Products.DTOs;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Product.Name,
            request.Product.Description,
            request.Product.Price,
            request.Product.Stock,
            clock.UtcNow);

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(ProductDto.FromEntity(product));
    }
}
