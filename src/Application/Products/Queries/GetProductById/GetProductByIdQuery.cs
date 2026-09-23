using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDto>>;
