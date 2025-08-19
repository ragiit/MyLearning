namespace Identity.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Produces("application/json")]
    public class AuthControllers(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Mengautentikasi pengguna dan mengembalikan token JWT.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///        "email": "admin@gmail.com",
        ///        "password": "P@ssw0rd"
        ///     }
        ///
        /// </remarks>
        /// <param name="request">Data kredensial login (email dan password).</param>
        /// <param name="cancellationToken">Token pembatalan.</param>
        /// <returns>Respons yang berisi token JWT jika login berhasil.</returns>
        [HttpPost("login")]
        // Mendokumentasikan kemungkinan respons HTTP yang dikembalikan oleh endpoint ini
        [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request);
            var loginResponse = await sender.Send(command, cancellationToken);

            var response = new BaseResponse<LoginResponse>(true, "Login successful", loginResponse.LoginResponse.Adapt<LoginResponse>());
            return Ok(response);
        }

        /// <summary>
        /// Mendaftarkan pengguna baru ke sistem.
        /// </summary>
        /// <param name="request">Data pendaftaran pengguna baru.</param>
        /// <param name="cancellationToken">Token pembatalan.</param>
        /// <returns>Respons yang mengindikasikan keberhasilan pendaftaran.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<RegisterResult>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<RegisterResult>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request);
            var registerResult = await sender.Send(command, cancellationToken);

            var response = new BaseResponse<RegisterResult>(true, "Registration successful", registerResult.Adapt<RegisterResult>());

            return CreatedAtAction(nameof(Register), new { id = registerResult.Id }, response);
        }

        // Endpoint Refresh, Logout, LogoutAll akan mengikuti pola yang sama

        // /// <summary>
        // /// Memperbarui token akses menggunakan refresh token.
        // /// </summary>
        // [HttpPost("refresh")]
        // [AllowAnonymous]
        // [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        // public async Task<ActionResult<BaseResponse<LoginResponse>>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        // {
        //     var loginResponse = await sender.Send(new RefreshTokenCommand(request), cancellationToken);
        //     return Ok(new BaseResponse<LoginResponse>("Token refreshed successfully", true, loginResponse));
        // }

        // /// <summary>
        // /// Mencabut refresh token pengguna saat ini.
        // /// </summary>
        // [HttpPost("logout")]
        // [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)] // Untuk operasi tanpa Result spesifik
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        // public async Task<ActionResult<BaseResponse>> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        // {
        //     await sender.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        //     return Ok(new BaseResponse("Logout successful", true));
        // }

        // /// <summary>
        // /// Mencabut semua refresh token untuk pengguna saat ini (logout dari semua perangkat).
        // /// </summary>
        // [HttpPost("logout-all")]
        // [Authorize] // Membutuhkan otentikasi
        // [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        // public async Task<ActionResult<BaseResponse>> LogoutAll(CancellationToken cancellationToken)
        // {
        //     var userId = User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException("User ID not found.");
        //     await sender.Send(new LogoutAllCommand(userId), cancellationToken);
        //     return Ok(new BaseResponse("Logged out from all devices", true));
        // }
    }

    // Anda perlu mendefinisikan RegisterResult di namespace Identity.API.Dtos
    // Contoh sederhana:
    public record RegisterResult(string Id, string Email);
}