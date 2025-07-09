using Apple.Services.RewardAPI.Message;

namespace Apple.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateReward(RewardMessage rewardMessage);
    }
}