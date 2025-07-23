namespace Auth.API.Persistence.Seed
{
    public static class IdentityRoles
    {
        public const string Admin = "Admin";
        public const string Cashier = "Cashier";
        public const string Chef = "Chef";
        public const string Customer = "Customer";

        public static readonly string[] DefaultRoles = [Admin, Cashier, Chef, Customer];
    }
}