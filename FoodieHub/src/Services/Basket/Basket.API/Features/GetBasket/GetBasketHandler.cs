namespace Basket.API.Features.GetBasket
{
    // QUERY
    public sealed record GetBasketQuery(string UserName) : IQuery<BasketDto>;

    // HANDLER
    public class GetBasketHandler(IBasketRepository repository) : IQueryHandler<GetBasketQuery, BasketDto>
    {
        public async Task<BasketDto> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasketAsync(request.UserName, cancellationToken)
                ?? throw new BasketNotFoundException(request.UserName);

            return basket;
        }
    }
}