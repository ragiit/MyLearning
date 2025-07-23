// Menu.API/Exceptions/MenuNotFoundException.cs
using BuildingBlocks.Exceptions; // Pastikan ini di-import

namespace Menu.API.Exceptions
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