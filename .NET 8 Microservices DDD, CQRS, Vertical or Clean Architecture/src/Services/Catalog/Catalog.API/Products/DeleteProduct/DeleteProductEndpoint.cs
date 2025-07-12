namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductRequest(Guid ProductId);
    public record DeleteProductResponse(bool IsSuccess);

    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{productId:guid}", async (Guid productId, ISender sender) =>
            {
                var command = new DeleteProductCommand(productId);
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteProductResponse>();
                return Results.Ok(response);
            })
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product by ID")
            .WithTags("Products");
        }
    }
}