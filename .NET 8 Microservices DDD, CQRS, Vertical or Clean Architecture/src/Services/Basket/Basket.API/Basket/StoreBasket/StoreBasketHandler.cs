using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(c => c.Cart).NotNull();
            RuleFor(c => c.Cart.UserName).NotEmpty();
        }
    }

    internal class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProtoService) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            await DeductDiscount(request, cancellationToken);

            await repository.StoreBasketAsync(request.Cart, cancellationToken);

            return new StoreBasketResult(request.Cart.UserName);
        }

        private async Task DeductDiscount(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Cart.Items)
            {
                var coupon = await discountProtoService.GetDiscountAsync(new GetDiscountRequest
                {
                    ProductName = item.ProductName,
                }, cancellationToken: cancellationToken);
                item.Price -= coupon.Amount;
            }
        }
    }
}