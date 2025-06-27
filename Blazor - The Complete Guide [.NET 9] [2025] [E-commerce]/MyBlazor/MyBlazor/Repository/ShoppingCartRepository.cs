using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Repository
{
    public class ShoppingCartRepository(ApplicationDbContext context) : IRepository.IShoppingCartRepository
    {
        public async Task<bool> UpdateCartAsync(string userId, int productId, int quantity)
        {
            if (userId == null)
                return false;

            var cartItem = await context.ShoppingCarts.FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                if (cartItem.Quantity <= 0)
                {
                    context.ShoppingCarts.Remove(cartItem);
                }
            }
            else
            {
                cartItem = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await context.ShoppingCarts.AddAsync(cartItem);
            }

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartAsync(string userId)
        {
            return await context.ShoppingCarts
                .Include(x => x.Product)
                .Include(x => x.ApplicationUser)
                .Where(c => c.ApplicationUserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartItems = await context.ShoppingCarts.Where(c => c.ApplicationUserId == userId).ToListAsync();
            if (cartItems.Count != 0)
            {
                context.ShoppingCarts.RemoveRange(cartItems);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}