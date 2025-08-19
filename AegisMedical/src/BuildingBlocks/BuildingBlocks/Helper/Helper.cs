namespace BuildingBlocks.Helper
{
    public static class Helper
    {
        public record BaseResponse<T>(bool IsSuccess, string Message, T? Result = default);
        public record BaseResponse(bool IsSuccess, string Message);

        public const string RoleAdmin = "Admin";
        public const string RoleEmployee = "Employee";
        public const string RoleCustomer = "Customer";
        public const string RoleChef = "Chef";

        public static string GetValueOrDefault(this string? value, string defaultValue = "-")
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}