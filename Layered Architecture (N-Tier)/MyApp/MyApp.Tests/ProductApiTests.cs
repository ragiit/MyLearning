using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MyApp.Domain.Entities;
using System.Net;
using System.Net.Http.Json;

namespace MyApp.Tests;

public class ProductApiTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("https://localhost:5000")
    };

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostProduct_ShouldCreateProduct()
    {
        // Arrange
        var newProduct = new Product { Name = "Integration Test Product", Price = 123 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}