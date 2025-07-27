namespace Auth.UnitTests.Features.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_LoginRequest_Is_Null()
    {
        // Arrange
        var command = new LoginCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest
        {
            Email = " ",
            Password = "password123"
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest
        {
            Email = "test@example.com",
            Password = " "
        });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var command = new LoginCommand(new LoginRequest { Email = "test@example.com", Password = "password123" });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}