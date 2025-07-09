using Apple.Services.RewardAPI.Message;
using Apple.Services.RewardAPI.Models;
using System.Text;

namespace Apple.Services.RewardAPI.Services
{
    public class RewardService(DbContextOptions<AppDbContext> options) : IRewardService
    {
        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "<html><body><h1>Welcome to Apple!</h1><p>Thank you for registering with us. We're excited to have you.</p></body></html>";
            await LogAndEmail(message, email);
        }

        public async Task UpdateReward(RewardMessage rewardMessage)
        {
            var reward = new Reward
            {
                UserId = rewardMessage.UserId,
                Date = DateTime.Now,
                RewardActivity = rewardMessage.RewardActivity,
                OrderId = rewardMessage.OrderId
            };

            await using var _db = new AppDbContext(options);
            await _db.Rewards.AddAsync(reward);
            await _db.SaveChangesAsync();
        }

        // Metode privat untuk menyimpan log ke DB dan (secara konseptual) mengirim email.
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                // Membuat instance DbContext menggunakan options yang sudah di-inject.
                await using var _db = new AppDbContext(options);

                //EmailLogger emailLog = new()
                //{
                //    Email = email,
                //    Message = message,
                //    SentAt = DateTime.Now
                //};

                //// Menyimpan log ke database.
                //await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                // TODO: Tambahkan logika untuk mengirim email menggunakan service
                // seperti SendGrid, MailKit, atau SMTP client lainnya.
                // Contoh: return await _emailSender.SendEmailAsync(email, "Your Cart Details", message);

                return true; // Asumsikan email berhasil dikirim.
            }
            catch (Exception)
            {
                // Jika terjadi error, proses dianggap gagal.
                return false;
            }
        }
    }
}