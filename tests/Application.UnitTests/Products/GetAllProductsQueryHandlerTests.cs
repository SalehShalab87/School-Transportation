using Application.Products.Abstractions;
using Application.Products.DTOs;
using Application.Products.Queries.GetAllProducts;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Products;

public sealed class GetAllProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Use_Query_Service()
    {
        var products = new List<ProductDto>
        {
            new(Guid.CreateVersion7(), "A", "D", 1m, 1, DateTimeOffset.UtcNow, null)
        };
        var readService = new Mock<IProductReadService>();
        readService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);
        var handler = new GetAllProductsQueryHandler(readService.Object);

        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        readService.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
