using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddlewares();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // Membatasi akses dashboard hanya di development, atau dengan Autorisasi khusus di produksi
    Authorization = [new HangfireDashboardAuthorizationFilter()]
});

app.MapControllers();
var cleanupIntervalMinutes = app.Configuration.GetValue<int>("BasketServiceSettings:AbandonedCartCleanupIntervalMinutes");

// --- PERBAIKAN DI SINI ---
string cronExpression;

if (cleanupIntervalMinutes > 0 && cleanupIntervalMinutes < 60)
{
    // Jika interval kurang dari 60 menit, gunakan Cron.MinuteInterval
    cronExpression = Cron.MinuteInterval(cleanupIntervalMinutes);
}
else if (cleanupIntervalMinutes == 60)
{
    // Jika tepat 60 menit (setiap jam), gunakan Cron.Hourly()
    cronExpression = Cron.Hourly();
}
else if (cleanupIntervalMinutes > 60 && cleanupIntervalMinutes % 60 == 0)
{
    // Jika kelipatan jam (misal 120 menit = setiap 2 jam)
    int hours = cleanupIntervalMinutes / 60;
    cronExpression = Cron.HourInterval(hours);
}
else
{
    // Default jika nilai tidak valid, misalnya setiap 15 menit sebagai fallback aman
    app.Logger.LogWarning("BasketServiceSettings:AbandonedCartCleanupIntervalMinutes ({cleanupIntervalMinutes}) tidak valid. Menggunakan default 'setiap 15 menit'.", cleanupIntervalMinutes);
    cronExpression = Cron.MinuteInterval(15);
}

RecurringJob.AddOrUpdate<AbandonedCartCleanupJob>(
    "BersihkanKeranjangKadaluarsa", // ID unik untuk job recurring
    job => job.CleanAbandonedCarts(),
    cronExpression); // Gunakan cronExpression yang sudah ditentukan
// --- AKHIR PERBAIKAN ---

app.Run();

//public static class SeedData
//{
//    public static async Task EnsureHangfireDatabaseExists(IConfiguration configuration)
//    {
//        var connectionString = configuration.GetConnectionString("HangfireConnection");
//        if (string.IsNullOrEmpty(connectionString))
//        {
//            throw new InvalidOperationException("HangfireConnection string is not configured.");
//        }

//        var builder = new SqlConnectionStringBuilder(connectionString);
//        var databaseName = builder.InitialCatalog; // Dapatkan nama database dari connection string
//        builder.InitialCatalog = "master"; // Ganti ke master database untuk membuat database baru

//        // Buat connection string ke master database
//        var masterConnectionString = builder.ConnectionString;

//        using (var connection = new SqlConnection(masterConnectionString))
//        {
//            await connection.OpenAsync();
//            var query = $"SELECT name FROM sys.databases WHERE name = N'{databaseName}'";
//            var dbExists = await connection.QueryFirstOrDefaultAsync<string>(query);

//            if (string.IsNullOrEmpty(dbExists))
//            {
//                Console.WriteLine($"ℹ️ Database '{databaseName}' tidak ditemukan. Mencoba membuat...");
//                // Buat database baru
//                await connection.ExecuteAsync($"CREATE DATABASE [{databaseName}]");
//                Console.WriteLine($"✅ Database '{databaseName}' berhasil dibuat.");
//            }
//            else
//            {
//                Console.WriteLine($"ℹ️ Database '{databaseName}' sudah ada.");
//            }
//        }
//    }

//    // Anda bisa tambahkan ApplyMigrationsAsync (untuk EF Core DB) di sini jika Anda juga punya di Basket API
//    // public static async Task ApplyMigrationsAsync(IServiceProvider services) { ... }
//}
//}