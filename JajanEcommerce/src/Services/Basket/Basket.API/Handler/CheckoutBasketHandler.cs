using Basket.API.Data;
using Basket.API.Dtos;
using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Events;
using Mapster;
using MassTransit;

namespace Basket.API.Handler
{
    public record CheckOutBasketCommand(string userId, BasketCheckoutRequestDto BasketCheckoutDto)
        : ICommand<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);

    public class CheckoutBasketCommandHandler
        (IBasketRepository repository, IPublishEndpoint publishEndpoint)
        : ICommandHandler<CheckOutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckOutBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasketAsync(command.userId, cancellationToken);
            if (basket == null)
            {
                return new CheckoutBasketResult(false);
            }

            // Mapping dari DTO
            var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
            eventMessage.Username = command.userId;
            eventMessage.Date = DateTime.Now;
            // Tambahkan items dari ShoppingCart
            eventMessage.Items = [.. basket.Items.Select(x => new BuildingBlocks.Messaging.Events.OrderItemDto
            {
                MenuItemId = x.MenuItemId,
                Quantity = x.Quantity,
                ProductName = x.ProductName,
                Price = x.Price,
            })];

            // Publish event
            await publishEndpoint.Publish(eventMessage, cancellationToken);

            return new CheckoutBasketResult(true);
        }
    }
}