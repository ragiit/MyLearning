using Refit;

namespace ShoppingWeb.Services
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<BaseResponse<LoginResponse>> Login([Body] LoginRequest request);
    }
}