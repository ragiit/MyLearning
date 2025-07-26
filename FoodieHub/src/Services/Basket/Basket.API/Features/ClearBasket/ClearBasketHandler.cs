namespace Basket.API.Features.ClearBasket
{
    // COMMAND
    public sealed record ClearBasketCommand(string UserName) : ICommand<Unit>;

    // HANDLER
    public class ClearBasketHandler(IBasketRepository repository) : ICommandHandler<ClearBasketCommand, Unit>
    {
        public async Task<Unit> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await repository.GetBasketAsync(request.UserName, cancellationToken);

            // Jika keranjang tidak ada, throw NotFoundException
            if (basket == null)
            {
                throw new BasketNotFoundException(request.UserName);
            }

            await repository.DeleteBasketAsync(request.UserName, cancellationToken);

            return Unit.Value;
        }
    }
}