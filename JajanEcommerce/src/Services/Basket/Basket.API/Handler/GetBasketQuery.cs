using Basket.API.Data;
using Basket.API.Models;
using MediatR;

namespace Basket.API.Handler
{
    public record GetBasketQuery(string UserName) : IRequest<ShoppingCart>;

    public class GetBasketQueryHandler(IBasketRepository repository) : IRequestHandler<GetBasketQuery, ShoppingCart>
    {
        public async Task<ShoppingCart> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetBasketAsync(request.UserName, cancellationToken);
        }
    }
}