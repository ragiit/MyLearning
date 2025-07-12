namespace Basket.API.Basket.GetBasket
{
    //public record GetBasketRequest(string UserName);
    public record GetBasketResponse(ShoppingCart Cart);

    public class GetBasketEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var query = new GetBasketQuery(userName);
                var result = await sender.Send(query);
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
            .WithName("GetBasket")
            .WithSummary("Get a user's shopping basket")
            .WithDescription("Retrieves the shopping basket for the specified user.")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Basket");
        }
    }
}