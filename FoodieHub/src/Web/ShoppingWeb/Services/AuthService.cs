using Microsoft.AspNetCore.Components;
using Refit;

namespace ShoppingWeb.Services
{
    public class AuthService
    {
        private readonly AccessTokenService _accessTokenService;
        private readonly NavigationManager _navigationManager;
        private readonly IAuthApi _authApi; // Injeksi IAuthApi (Refit client)
        private readonly HttpClient _httpClient; // HttpClient bisa digunakan untuk hal lain yang tidak pakai Refit

        public AuthService(
            AccessTokenService accessTokenService,
            NavigationManager navigationManager,
            HttpClient httpClient, // HttpClient yang sudah dikonfigurasi (misal "AuthorizedApi")
            IAuthApi authApi) // Injeksi IAuthApi
        {
            _accessTokenService = accessTokenService;
            _navigationManager = navigationManager;
            _httpClient = httpClient;
            _authApi = authApi;
        }

        // --- FUNGSI LOGIN ---
        public async Task<LoginResponse?> Login(LoginRequest loginRequest)
        {
            try
            {
                // Panggil API login menggunakan Refit
                BaseResponse<LoginResponse> apiResponse = await _authApi.Login(loginRequest);

                if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Result == null)
                {
                    throw new Exception(apiResponse?.Message ?? "Login gagal dari API.");
                }

                // Set token ke cookie menggunakan AccessTokenService
                await _accessTokenService.SetToken(apiResponse.Result.Token);

                // Secara opsional, atur header Authorization default untuk HttpClient jika diperlukan
                // untuk panggilan API lain yang tidak menggunakan Refit.
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiResponse.Result.Token);

                return apiResponse.Result; // Kembalikan objek LoginResponse
            }
            catch (ApiException ex) // Tangkap Refit-specific exceptions
            {
                var errorContent = await ex.GetContentAsAsync<BaseResponse>();
                throw new Exception(errorContent?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Login gagal: {ex.Message}");
            }
        }

        // --- FUNGSI LOGOUT ---
        public async Task Logout()
        {
            await _accessTokenService.RemoveToken();
            // Hapus header Authorization default dari HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = null;
            // Redirect ke halaman login setelah logout
            _navigationManager.NavigateTo("/login");
        }

        // --- FUNGSI UNTUK MENDAPATKAN STATUS LOGIN (opsional) ---
        public async Task<bool> IsUserLoggedIn()
        {
            string? token = await _accessTokenService.GetToken();
            return !string.IsNullOrEmpty(token);
        }
    }
}