using Auth.API.Dtos;
using Auth.API.Handler;
using BuildingBlocks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Auth.API.Controllers;

[Route("api/auth")]
[ApiController]
[EnableRateLimiting("fixed")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login(LoginRequestDto request)
    {
        var command = new AuthCommand(request);
        var result = await sender.Send(command);

        var response = new ResponseDto<LoginResponseDto>
        {
            IsSuccess = result.LoginResponse.User != null,
            Result = result.LoginResponse,
            Message = result.LoginResponse.User is null ? "Invalid username or password." : ""
        };

        if (response.IsSuccess)
            return Ok(response);

        return Unauthorized(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ResponseDto<RegisterResponseDto>>> Register(RegisterRequestDto request)
    {
        var command = new RegisterCommand(request);
        var result = await sender.Send(command);

        var response = new ResponseDto<RegisterResponseDto>
        {
            IsSuccess = result.RegisterResponse.User != null,
            Result = result.RegisterResponse,
            Message = result.RegisterResponse.User is null ? "Registration failed." : ""
        };

        if (response.IsSuccess)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpGet("users")]
    [Authorize(Roles = $"{Helper.Helper.RoleAdmin}")]
    public async Task<ActionResult<ResponseDto<List<UserDto>>>> GetAllUsers()
    {
        var result = await sender.Send(new GetAllUsersQuery());

        var response = new ResponseDto<List<UserDto>>
        {
            IsSuccess = true,
            Result = result.Users
        };

        return Ok(response);
    }

    [HttpGet("test")]
    public async Task<ActionResult<ResponseDto<List<UserDto>>>> Test()
    {
        var result = await sender.Send(new GetAllUsersQuery(true));

        var response = new ResponseDto<List<UserDto>>
        {
            IsSuccess = true,
            Result = result.Users
        };
        return Ok();
    }
}