using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Apple.Web.Controllers
{
    public class AuthController(IAuthService authService, ITokenProvider tokenProvider) : Controller
    {
        // Action GET untuk menampilkan halaman login.
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        // Action POST untuk memproses login.
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid) return View(model);

            ResponseDto? responseDto = await authService.LoginAsync(model);

            if (responseDto != null && responseDto.IsSuccess)
            {
                // Jika login sukses, deserialisasi respons dan tandai user sebagai login (sign in).
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                tokenProvider.SetToken(loginResponseDto.Token);

                await SignInUser(loginResponseDto);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto?.Message ?? "An error occurred.");
                return View(model);
            }
        }

        // Action GET untuk menampilkan halaman registrasi.
        [HttpGet]
        public IActionResult Register()
        {
            // Menyiapkan daftar role untuk dropdown di view.
            var roleList = new List<SelectListItem>()
            {
                new() { Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new() {Text = SD.RoleCustomer, Value = SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View(new RegistrationRequestDto());
        }

        // Action POST untuk memproses registrasi.
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            if (!ModelState.IsValid) return View(model);

            ResponseDto? result = await authService.RegisterAsync(model);

            if (result != null && result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(model.Role))
                {
                    // INI ADALAH TEMPAT PENGGUNAANNYA
                    await authService.AssignRoleAsync(model);
                }

                // Jika registrasi sukses, redirect ke halaman login dengan pesan sukses.
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ModelState.AddModelError("CustomError", result?.Message ?? "An error occurred.");
                return View(model);
            }
        }

        // Action GET untuk logout.
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        // Metode helper untuk menandai user sebagai login berbasis cookie
        // setelah mendapatkan JWT token dari API.
        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? string.Empty));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}