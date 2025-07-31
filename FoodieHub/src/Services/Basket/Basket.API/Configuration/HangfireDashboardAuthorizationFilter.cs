using Hangfire.Dashboard;

namespace Basket.API.Configuration
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
            var httpContext = context.GetHttpContext();
            // Contoh: Hanya izinkan user dengan role Admin untuk mengakses dashboard
            // Pastikan user terautentikasi dan memiliki role Admin
            return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole("Admin");

            // Di development, Anda bisa return true untuk akses mudah:
            // return true;
        }
    }
}