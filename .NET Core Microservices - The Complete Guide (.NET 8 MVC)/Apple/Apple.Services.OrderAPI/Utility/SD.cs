﻿namespace Apple.Services.OrderAPI.Utility
{
    public static class SD
    {
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusReadyForPickup = "ReadyForPickup";
        public const string StatusCancelled = "Cancelled";
        public const string StatusCompleted = "Completed";
        public const string StatusRefunded = "Refunded";

        public const string SessionCart = "SessionCart";

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
    }
}