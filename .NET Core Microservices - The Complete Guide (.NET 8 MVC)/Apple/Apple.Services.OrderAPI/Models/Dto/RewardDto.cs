namespace Apple.Services.OrderAPI.Models.Dto
{
    public class RewardDto
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int RewardActivity { get; set; }
        public int OrderId { get; set; }
    }
}