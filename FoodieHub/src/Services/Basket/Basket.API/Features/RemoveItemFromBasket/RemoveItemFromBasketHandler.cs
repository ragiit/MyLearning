namespace Basket.API.Features.RemoveItemFromBasket
{
    // COMMAND
    public sealed record RemoveItemFromBasketCommand(RemoveItemFromBasketRequest Request, string UserName) : ICommand<BasketDto>;

    // VALIDATOR
    public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
    {
        public RemoveItemFromBasketCommandValidator()
        {
            RuleFor(x => x.Request.MenuId).NotEmpty().WithMessage("MenuId is required.");
        }
    }

    // HANDLER
    public class RemoveItemFromBasketHandler(IBasketRepository repository) : ICommandHandler<RemoveItemFromBasketCommand, BasketDto>
    {
        public async Task<BasketDto> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasketAsync(request.UserName, cancellationToken)
                ?? throw new BasketNotFoundException(request.UserName);

            var existingItem = basket.Items.FirstOrDefault(item => item.MenuId == request.Request.MenuId);

            if (existingItem == null)
            {
                throw new BasketItemNotFoundException(request.Request.MenuId);
            }

            basket.Items.Remove(existingItem);

            // Recalculate total price
            basket = basket with { TotalPrice = basket.Items.Sum(item => item.Price * item.Quantity) };

            // Store updated basket
            await repository.StoreBasketAsync(basket, cancellationToken);

            return basket;
        }
    }
}