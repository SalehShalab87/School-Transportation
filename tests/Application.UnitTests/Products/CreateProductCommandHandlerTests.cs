using Application.Common.Abstractions;
using Application.Common.Abstractions.Persistence;
using Application.Products.Commands.CreateProduct;
using Application.Products.DTOs;
using Domain.Products;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.Products;

public sealed class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_Aggregate_Then_Commit_Once()
    {
        var now = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var clock = new Mock<IClock>();
        clock.SetupGet(x => x.UtcNow).Returns(now);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(repository.Object, unitOfWork.Object, clock.Object);
        var command = new CreateProductCommand(new CreateProductDto("Router", "Description", 99m, 5));

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.CreatedAtUtc.Should().Be(now);
        repository.Verify(x => x.Add(It.Is<Product>(p => p.Name == "Router")), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
