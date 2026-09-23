using Application.Common;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.DTOs;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Queries.GetProductById;
using MediatR;

namespace API.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/products")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapGet("/", GetAllAsync).WithName("GetAllProducts");
        group.MapGet("/{id:guid}", GetByIdAsync).WithName("GetProductById");
        group.MapPost("/", CreateAsync).WithName("CreateProduct");
        group.MapPut("/{id:guid}", UpdateAsync).WithName("UpdateProduct");
        group.MapDelete("/{id:guid}", DeleteAsync).WithName("DeleteProduct");

        return group;
    }

    private static async Task<IResult> GetAllAsync(ISender sender, CancellationToken cancellationToken) =>
        (await sender.Send(new GetAllProductsQuery(), cancellationToken)).ToHttpResult();

    private static async Task<IResult> GetByIdAsync(Guid id, ISender sender, CancellationToken cancellationToken) =>
        (await sender.Send(new GetProductByIdQuery(id), cancellationToken)).ToHttpResult();

    private static async Task<IResult> CreateAsync(
        CreateProductDto dto,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateProductCommand(dto), cancellationToken);
        return result.IsSuccess
            ? Results.Created($"/api/products/{result.Data!.Id}", result)
            : result.ToHttpResult();
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateProductDto dto,
        ISender sender,
        CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateProductCommand(id, dto), cancellationToken)).ToHttpResult();

    private static async Task<IResult> DeleteAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken) =>
        (await sender.Send(new DeleteProductCommand(id), cancellationToken)).ToHttpResult();
}
