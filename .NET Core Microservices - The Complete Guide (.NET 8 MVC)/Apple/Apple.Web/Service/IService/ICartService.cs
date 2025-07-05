namespace Apple.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);

        Task<ResponseDto?> UspsertCartAsync(CartDto cartHeaderDto);

        Task<ResponseDto?> RemoveCartAsync(int cartDetailId);
        Task<ResponseDto?> RemoveCouponAsync(CartDto cartDto);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cart);
    }
}