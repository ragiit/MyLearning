namespace Apple.Services.EmailAPI.Models.Dto
{
    public class EmailLoggerDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}