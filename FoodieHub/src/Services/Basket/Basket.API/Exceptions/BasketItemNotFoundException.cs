namespace Basket.API.Exceptions
{
    public class BasketItemNotFoundException(Guid menuId) : NotFoundException("Basket Item", menuId.ToString())
    {
    }
}