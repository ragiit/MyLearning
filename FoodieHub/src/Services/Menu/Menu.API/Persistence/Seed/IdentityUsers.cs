namespace Auth.API.Persistence.Seed
{
    public static class IdentityUsers
    {
        public static readonly (string Role, string Email, string Name)[] DefaultUsers =
        [
            (IdentityRoles.Admin, "admin@gmail.com", "System Administrator"),
        (IdentityRoles.Cashier, "cashier@gmail.com", "Cashier User"),
        (IdentityRoles.Chef, "chef@gmail.com", "Chef User"),
        (IdentityRoles.Customer, "customer@gmail.com", "Customer User")
        ];
    }
}