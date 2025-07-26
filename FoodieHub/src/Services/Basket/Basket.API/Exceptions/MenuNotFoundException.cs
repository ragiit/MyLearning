// Menu.API/Exceptions/MenuNotFoundException.cs

// Menu.API/Exceptions/MenuNotFoundException.cs
namespace Basket.API.Exceptions
{
    public class MenuNotFoundException : NotFoundException
    {
        public MenuNotFoundException(Guid menuId) : base("Menu", menuId.ToString())
        {
        }

        public MenuNotFoundException(string message) : base("Menu", message)
        {
        }
    }
}