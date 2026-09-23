using Application.Common;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IClock clock)
    : IRequestHandler<DeleteProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result<Guid>.Failure(Error.NotFound("Products.NotFound", "Product was not found."));

        product.SoftDelete(clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }
}
