using Apple.Services.EmailAPI.Message;
using System.Text;

namespace Apple.Services.EmailAPI.Services
{
    public class EmailService(DbContextOptions<AppDbContext> options) : IEmailService
    {
        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new();

            // Membangun konten email dengan format HTML yang rapi.
            message.AppendLine("<html><body>");
            message.AppendLine("<h1>Your Cart Details</h1>");
            message.AppendLine("<p>Here is a summary of your shopping cart.</p>");
            message.AppendLine("<br/>");

            message.Append("<h3>Order Summary</h3>");
            message.Append("<table border='1' style='width:100%; border-collapse: collapse;'>");
            message.Append("<thead><tr><th style='padding: 8px;'>Product</th><th style='padding: 8px;'>Quantity</th></tr></thead>");
            message.Append("<tbody>");

            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<tr>");
                message.Append($"<td style='padding: 8px;'>{item.Product?.Name ?? "N/A"}</td>");
                message.Append($"<td style='padding: 8px; text-align: center;'>{item.Count}</td>");
                message.Append("</tr>");
            }
            message.Append("</tbody></table>");

            message.Append($"<h3 style='text-align: right;'>Total: {cartDto.CartHeader.CartTotal:C}</h3>");
            message.AppendLine("</body></html>");

            // Memanggil metode privat untuk logging dan pengiriman.
            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardMessage rewardMessage)
        {
            string message = "New Order Placed. Order ID: " + rewardMessage.OrderId;
            await LogAndEmail(message, "argi@gmail.com");
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "<html><body><h1>Welcome to Apple!</h1><p>Thank you for registering with us. We're excited to have you.</p></body></html>";
            await LogAndEmail(message, email);
        }

        // Metode privat untuk menyimpan log ke DB dan (secara konseptual) mengirim email.
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                // Membuat instance DbContext menggunakan options yang sudah di-inject.
                await using var _db = new AppDbContext(options);

                EmailLogger emailLog = new()
                {
                    Email = email,
                    Message = message,
                    SentAt = DateTime.Now
                };

                // Menyimpan log ke database.
                await _db.EmailLoggers.AddAsync(emailLog);
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