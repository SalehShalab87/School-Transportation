using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(Guid ProductId, UpdateProductDto Product) : IRequest<Result<ProductDto>>;
