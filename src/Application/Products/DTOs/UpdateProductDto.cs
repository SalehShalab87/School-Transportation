namespace Application.Products.DTOs;

public sealed record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock);
