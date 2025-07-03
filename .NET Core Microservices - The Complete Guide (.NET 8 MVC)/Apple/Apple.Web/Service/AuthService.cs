using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Apple.Web.Service
{
    public class AuthService(IBaseService baseService, IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            });
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto assignRoleRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = assignRoleRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assign-role"
            });
        }

        public async Task LogoutAsync()
        {
            // Menghapus cookie otentikasi dari browser client
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}