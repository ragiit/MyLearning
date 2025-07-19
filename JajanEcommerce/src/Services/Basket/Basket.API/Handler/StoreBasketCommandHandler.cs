using Basket.API.Data;
using Basket.API.Exceptions;
using Basket.API.Models;
using Basket.API.Services;
using MediatR;
using Refit;
using System.Net;

namespace Basket.API.Handler
{
    public record StoreBasketCommand(string UserName, List<ShoppingCartItemDto> Items) : IRequest<ShoppingCart>;

    public class StoreBasketCommandHandler(IBasketRepository repository, IMenuApi menuApi) : IRequestHandler<StoreBasketCommand, ShoppingCart>
    {
        public async Task<ShoppingCart> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = new ShoppingCart { UserName = request.UserName };
            var notFoundIds = new List<Guid>();

            foreach (var item in request.Items)
            {
                try
                {
                    var menuItem = await menuApi.GetMenuByIdAsync(item.MenuItemId);
                    if (menuItem.IsSuccess)
                    {
                        basket.Items.Add(new ShoppingCartItem
                        {
                            MenuItemId = item.MenuItemId,
                            ProductName = menuItem.Result!.Name,
                            Quantity = item.Quantity,
                            Price = menuItem.Result.Price
                        });
                    }
                }
                catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    notFoundIds.Add(item.MenuItemId);
                }
            }

            if (notFoundIds.Count > 0)
            {
                throw new BasketNotFoundException($"MenuItemIds not found: {string.Join(", ", notFoundIds)}");
            }

            return await repository.StoreBasketAsync(basket, cancellationToken);
        }
    }
}