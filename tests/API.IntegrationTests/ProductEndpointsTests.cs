using System.Net;
using System.Net.Http.Json;
using Application.Common;
using Application.Products.DTOs;
using FluentAssertions;

namespace API.IntegrationTests;

public sealed class ProductEndpointsTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_Should_Return_Created()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductDto("Router", "Description", 49.99m, 10));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<Result<ProductDto>>();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Invalid_Create_Should_Return_BadRequest_From_Validation_Pipeline()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductDto("", "Description", 10m, 1));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Missing_Product_Should_Return_NotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.CreateVersion7()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
