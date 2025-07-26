using Refit;

namespace Basket.API.Services
{
    public interface IMenuApiClient
    {
        [Get("/api/menus/{menuId}")]
        Task<BaseResponse<MenuDto>> GetMenuById(Guid menuId, [Header("Authorization")] string authorization, CancellationToken cancellationToken = default);
    }
}