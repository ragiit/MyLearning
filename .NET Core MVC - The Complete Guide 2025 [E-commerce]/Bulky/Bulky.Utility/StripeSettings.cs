namespace Bulky.Utility
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string SessionUrl { get; set; } = string.Empty;
        public string SessionSuccessUrl { get; set; } = string.Empty;
        public string SessionCancelUrl { get; set; } = string.Empty;
    }
}