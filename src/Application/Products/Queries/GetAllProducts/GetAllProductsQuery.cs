using Application.Common;
using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery : IRequest<Result<IReadOnlyList<ProductDto>>>;
