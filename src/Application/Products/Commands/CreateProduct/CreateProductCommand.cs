using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Product) : IRequest<Result<ProductDto>>;
