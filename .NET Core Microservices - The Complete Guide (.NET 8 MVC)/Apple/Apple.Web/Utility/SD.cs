namespace Apple.Web.Utility
{
    public class SD
    {
        public const string RoleAdmin = "ADMIN";
        public static string RoleCustomer = "CUSTOMER";
        public static string TokenCookie = "JWTToken";
        public static string? CouponAPIBase { get; set; }
        public static string? AuthAPIBase { get; set; }
        public static string? ProductAPIBase { get; set; }
        public static string? ShoppingCartAPIBase { get; set; }
        public static string? OrderAPIBase { get; set; }
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusReadyForPickup = "ReadyForPickup";
        public const string StatusCancelled = "Cancelled";
        public const string StatusCompleted = "Completed";
        public const string StatusRefunded = "Refunded";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}