using Application.Common;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Result<Guid>>;
