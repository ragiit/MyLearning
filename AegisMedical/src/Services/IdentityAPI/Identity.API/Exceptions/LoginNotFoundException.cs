namespace Identity.API.Exceptions
{
    public class LoginNotFoundException(string message) : NotFoundException("ApplicationUser", message)
    {
    }
}