// Basket.API/Configuration/BasketServiceSettings.cs
namespace Basket.API.Configuration
{
    public class BasketServiceSettings
    {
        public const string SettingsSection = "BasketServiceSettings";

        public int AbandonedCartCleanupIntervalMinutes { get; set; } = 60;
        public int AbandonedCartExpirationDays { get; set; } = 7;
    }
}