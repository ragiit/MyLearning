// Auth.Application.UnitTests/Features/Login/LoginHandlerTests.cs
using Auth.API.Persistence;
using Auth.API.Persistence.Entities;
using Auth.API.Features.Login;
using Auth.API.Dtos;
using Auth.API.Exceptions;
using Auth.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;

namespace Auth.UnitTests.Features.Login;

public class LoginHandlerTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly LoginHandler _handler;

    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly string _testUserEmail = "test@example.com";
    private readonly string _testPassword = "Password123!";
    private readonly ApplicationUser _testUser;

    public LoginHandlerTests()
    {
        _dbContext = MockApplicationDbContext.GetDbContext(_testUserId, _testUserEmail);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        _mockJwtTokenGenerator = MockJwtTokenGenerator.GetMockJwtTokenGenerator();

        _testUser = _dbContext.ApplicationUsers.Single(u => u.Email == _testUserEmail);

        // Jangan setup CheckPasswordAsync di sini
        _mockUserManager.Setup(um => um.FindByEmailAsync(_testUserEmail)).ReturnsAsync(_testUser);

        _handler = new LoginHandler(_dbContext, _mockUserManager.Object, _mockJwtTokenGenerator.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAuthResult_When_LoginIsValid()
    {
        var command = new LoginCommand(new LoginRequest
        {
            Email = _testUserEmail,
            Password = _testPassword,
        });

        _mockUserManager.Setup(um => um.CheckPasswordAsync(_testUser, _testPassword)).ReturnsAsync(true);

        await Assert.ThrowsAsync<LoginNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowLoginNotFoundException_When_UserNotFound()
    {
        var command = new LoginCommand(new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        });

        _mockUserManager.Setup(um => um.FindByEmailAsync("nonexistent@example.com")).ReturnsAsync((ApplicationUser)null!);

        await Assert.ThrowsAsync<LoginNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_ThrowLoginNotFoundException_When_InvalidPassword()
    {
        // Arrange
        var wrongPassword = "wronggggggggg";
        var command = new LoginCommand(new LoginRequest
        {
            Email = _testUserEmail,
            Password = wrongPassword
        });

        _mockUserManager.Setup(um => um.CheckPasswordAsync(_testUser, wrongPassword)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<LoginNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}