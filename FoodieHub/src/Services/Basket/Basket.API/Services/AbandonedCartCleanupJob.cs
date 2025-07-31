// Basket.API/Services/AbandonedCartCleanupJob.cs
using Basket.API.Configuration;
using Basket.API.Data;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis; // Jika perlu akses langsung ke Redis untuk SCAN
using System;
using System.Linq;
using System.Text.Json; // Untuk serialisasi/deserialisasi BasketDto
using System.Threading.Tasks;

namespace Basket.API.Services
{
    // Job ini tidak perlu mengimplementasikan IHostedService
    public class AbandonedCartCleanupJob
    {
        private readonly ILogger<AbandonedCartCleanupJob> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _cartExpirationTime;

        // Injeksi IConnectionMultiplexer untuk akses Redis, dan IOptions untuk pengaturan
        public AbandonedCartCleanupJob(
            ILogger<AbandonedCartCleanupJob> logger,
            IConnectionMultiplexer redis,
            IOptions<BasketServiceSettings> settings)
        {
            _logger = logger;
            _redis = redis;
            _cartExpirationTime = TimeSpan.FromDays(settings.Value.AbandonedCartExpirationDays);
        }

        // Metode ini akan dipanggil oleh Hangfire
        [AutomaticRetry(Attempts = 0)] // Jangan otomatis retry jika gagal
        public async Task CleanAbandonedCarts()
        {
            _logger.LogInformation("Memulai siklus pembersihan keranjang kadaluarsa oleh Hangfire pada: {time}", DateTimeOffset.Now);

            var database = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            int cleanedCount = 0;
            // Gunakan pola wildcard yang spesifik jika Anda punya prefix untuk key keranjang
            // Misalnya: "user:*" atau "basket:*"
            await foreach (var key in server.KeysAsync(pattern: "*"))
            {
                // Anda bisa menambahkan CancellationToken jika Anda ingin job bisa dibatalkan secara eksternal,
                // tapi Hangfire sendiri punya mekanisme timeout dan retry.
                // Jika job ini dijalankan terlalu lama, mungkin perlu dipecah atau dioptimasi.

                var basketData = await database.StringGetAsync(key);
                if (!basketData.IsNullOrEmpty)
                {
                    var basket = JsonSerializer.Deserialize<BasketDto>(basketData);
                    if (basket != null)
                    {
                        // --- PERBAIKAN LOGIKA PERBANDINGAN DI SINI ---
                        // Pastikan basket.LastUpdated diperlakukan sebagai UTC
                        var lastUpdatedUtc = basket.LastUpdated.Kind == DateTimeKind.Utc ? basket.LastUpdated : basket.LastUpdated.ToUniversalTime();

                        // Waktu saat ini dalam UTC
                        var nowUtc = DateTime.UtcNow;

                        // Hitung selisih waktu
                        var timeSinceLastUpdate = nowUtc - lastUpdatedUtc;

                        // Kondisi untuk menghapus keranjang
                        // Jika _cartExpirationTime adalah TimeSpan.Zero, maka setiap keranjang yang
                        // LastUpdated-nya bukan persis NowUtc akan dianggap kadaluarsa.
                        // Jika ingin menghapus keranjang yang *lebih tua* dari waktu kadaluarsa,
                        // gunakan ">". Jika ingin menghapus keranjang yang *setua atau lebih tua*, gunakan ">=".
                        if (timeSinceLastUpdate >= _cartExpirationTime)
                        {
                            await database.KeyDeleteAsync(key);
                            _logger.LogInformation(
                                "Keranjang kadaluarsa untuk pengguna {UserName} dihapus. Terakhir diperbarui: {LastUpdated}, Kadaluarsa setelah: {ExpirationTime}",
                                key.ToString(), lastUpdatedUtc, _cartExpirationTime);
                            cleanedCount++;
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Keranjang untuk pengguna {UserName} masih aktif. Terakhir diperbarui: {LastUpdated}, Kadaluarsa setelah: {ExpirationTime}, Sisa waktu: {RemainingTime}",
                                key.ToString(), lastUpdatedUtc, _cartExpirationTime, _cartExpirationTime - timeSinceLastUpdate);
                        }
                        // --- AKHIR PERBAIKAN LOGIKA ---
                    }
                }
            }
            _logger.LogInformation("Siklus pembersihan keranjang kadaluarsa selesai. {Count} keranjang kadaluarsa dihapus.", cleanedCount);
        }
    }
}