using FluentAssertions;
using Moq;
using MyApp.Business.Services;
using MyApp.Data.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = 1, Name = "Test", Price = 10 } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        var product = new Product { Id = 1, Name = "Test" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task CreateAsync_ShouldCallAddAsync()
    {
        var product = new Product { Id = 1, Name = "New" };
        _repositoryMock.Setup(r => r.AddAsync(product)).ReturnsAsync(product);

        var result = await _service.CreateAsync(product);

        result.Should().BeEquivalentTo(product);
        _repositoryMock.Verify(r => r.AddAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteAsync()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}