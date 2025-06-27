namespace MyBlazor.Repository.IRepository
{
    public interface IShoppingCartRepository
    {
        public Task<bool> UpdateCartAsync(string userId, int productId, int quantity);

        public Task<IEnumerable<ShoppingCart>> GetCartAsync(string userId);

        public Task<bool> ClearCartAsync(string userId);
    }
}