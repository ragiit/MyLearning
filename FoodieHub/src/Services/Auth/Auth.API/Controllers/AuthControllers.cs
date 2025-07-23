using Auth.API.Dtos;
using Auth.API.Features.Login;
using Auth.API.Features.Logout;
using Auth.API.Features.RefreshToken;
using Auth.API.Features.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AuthControllers(ISender sender) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request);
            var result = await sender.Send(command, cancellationToken);

            return Ok(result.LoginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request);
            var result = await sender.Send(command, cancellationToken);

            return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
        }

        //[HttpPost("refresh")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await sender.Send(new RefreshTokenCommand(request), cancellationToken);
        //    return Ok(result);
        //}

        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        //{
        //    await sender.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        //    return NoContent();
        //}

        //[HttpPost("logout-all")]
        //[Authorize]
        //public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
        //{
        //    var userId = User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException();
        //    await sender.Send(new LogoutAllCommand(userId), cancellationToken);
        //    return NoContent();
        //}
    }
}