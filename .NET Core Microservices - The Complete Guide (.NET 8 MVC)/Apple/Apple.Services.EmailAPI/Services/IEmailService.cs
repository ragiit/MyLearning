namespace Apple.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cart);

        Task RegisterUserEmailAndLog(string email);
    }
}