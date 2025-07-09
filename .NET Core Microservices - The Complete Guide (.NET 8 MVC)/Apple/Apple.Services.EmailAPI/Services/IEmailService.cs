using Apple.Services.EmailAPI.Message;

namespace Apple.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cart);

        Task RegisterUserEmailAndLog(string email);

        Task LogOrderPlaced(RewardMessage rewardMessage);
    }
}