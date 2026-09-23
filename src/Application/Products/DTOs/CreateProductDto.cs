namespace Application.Products.DTOs;

public sealed record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock);
