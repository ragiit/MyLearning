using Microsoft.AspNetCore.Components;
using Stripe;
using Stripe.Checkout;

namespace MyBlazor.Services;

public class PaymentService(
    NavigationManager navigationManager,
    IOrderRepository orderRepository,
    ILogger<PaymentService> logger)
{
    public async Task<OrderHeader> CheckPaymentStatusAsync(OrderHeader orderHeader)
    {
        // Validasi input dasar
        if (orderHeader == null || string.IsNullOrEmpty(orderHeader.SessionId))
        {
            logger.LogError("CheckPaymentStatusAsync dipanggil dengan OrderHeader yang tidak valid.");
            return orderHeader;
        }
        try
        {
            var service = new SessionService();
            // Menggunakan metode GetAsync yang asinkron
            Session session = await service.GetAsync(orderHeader.SessionId);
            // Memeriksa status sesi pembayaran
            if (session.PaymentStatus == "paid")
            {
                // Update status order jika pembayaran berhasil
                orderHeader.Status = "Paid";
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                await orderRepository.UpdateStatusAsync(orderHeader.Id, orderHeader.Status, orderHeader.PaymentIntentId);
            }
            else if (session.PaymentStatus == "unpaid")
            {
                // Update status order jika pembayaran belum dilakukan
                orderHeader.Status = "Unpaid";
                await orderRepository.UpdateStatusAsync(orderHeader.Id, orderHeader.Status, null);
            }
            else
            {
                // Status lain bisa ditangani sesuai kebutuhan
                logger.LogWarning("Status pembayaran tidak dikenali: {PaymentStatus}", session.PaymentStatus);
            }
        }
        catch (StripeException e)
        {
            // Log error Stripe dengan detail untuk debugging
            logger.LogError(e, "Stripe API error saat memeriksa status pembayaran. Stripe Error: {StripeError}", e.StripeError.Message);
        }
        catch (Exception ex)
        {
            // Menangkap error umum lainnya
            logger.LogError(ex, "Terjadi error tidak terduga saat memeriksa status pembayaran.");
        }
        return orderHeader;
    }

    public async Task<OrderHeader?> FulfillOrderAsync(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            logger.LogError("FulfillOrderAsync dipanggil dengan sessionId kosong.");
            return null;
        }

        try
        {
            // 1. Ambil detail sesi dari Stripe untuk verifikasi
            var service = new SessionService();
            Session session = await service.GetAsync(sessionId);

            // 2. Periksa apakah pembayaran berhasil
            if (session.PaymentStatus == "paid")
            {
                // 3. Gunakan metode yang sudah Anda buat untuk mengambil order berdasarkan SessionId
                var orderHeader = await orderRepository.GetOrderBySessionIdAsync(sessionId);

                if (orderHeader != null)
                {
                    // 4. Panggil metode UpdateStatusAsync untuk memperbarui status dan PaymentIntentId
                    var updatedOrder = await orderRepository.UpdateStatusAsync(
                        orderHeader.Id,
                        "Pembayaran Berhasil", // Status baru
                        session.PaymentIntentId // PaymentIntentId dari sesi Stripe
                    );

                    logger.LogInformation("Pesanan #{OrderId} berhasil dibayar dan status diupdate.", orderHeader.Id);
                    return updatedOrder; // Kembalikan order yang sudah terupdate
                }
                else
                {
                    logger.LogWarning("Pembayaran Stripe berhasil untuk sesi {SessionId}, tetapi pesanan tidak ditemukan di database.", sessionId);
                }
            }
            else
            {
                logger.LogWarning("Pembayaran Stripe untuk sesi {SessionId} belum selesai. Status: {PaymentStatus}", sessionId, session.PaymentStatus);
            }
        }
        catch (StripeException e)
        {
            logger.LogError(e, "Stripe API error saat memverifikasi sesi {SessionId}: {StripeError}", sessionId, e.StripeError.Message);
        }
        catch (KeyNotFoundException knfEx)
        {
            // Menangkap error spesifik dari GetOrderBySessionIdAsync jika order tidak ditemukan
            logger.LogError(knfEx, "Error saat FulfillOrderAsync: {Message}", knfEx.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Terjadi error tidak terduga saat memverifikasi sesi {SessionId}", sessionId);
        }

        return null; // Kembalikan null jika ada masalah atau pembayaran tidak berhasil
    }

    public async Task<Session?> CreateStripeCheckoutSessionAsync(OrderHeader orderHeader, string domain)
    {
        // Validasi input dasar
        if (orderHeader == null || orderHeader.OrderDetails == null || !orderHeader.OrderDetails.Any())
        {
            logger.LogError("CreateStripeCheckoutSessionAsync dipanggil dengan OrderHeader yang tidak valid.");
            return null;
        }

        var options = new SessionCreateOptions
        {
            // URL diatur dengan benar
            SuccessUrl = $"{navigationManager.BaseUri}order/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{navigationManager.BaseUri}/order/cancel",

            // Menggunakan Select untuk memetakan OrderDetails ke LineItems
            LineItems = orderHeader.OrderDetails.Select(od => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    // Mata uang bisa dibuat dinamis jika perlu
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = od.Product.Name,
                        // Anda bisa menambahkan deskripsi atau gambar produk di sini jika perlu
                        // Description = od.Product.ShortDescription,
                        // Images = new List<string> { od.Product.PrimaryImageUrl }
                    },
                    // Konversi harga ke unit terkecil (sen untuk USD) sudah benar
                    UnitAmount = (long)(od.Product.Price * 100),
                },
                Quantity = od.Count,
            }).ToList(),

            Mode = "payment",

            // Menyimpan ID OrderHeader di metadata Stripe sangat direkomendasikan
            // Ini membantu Anda mencocokkan sesi Stripe dengan order di sistem Anda saat webhook diterima.
            Metadata = new Dictionary<string, string>
            {
                { "order_id", orderHeader.Id.ToString() }
            }
        };

        try
        {
            var service = new SessionService();
            // Menggunakan metode CreateAsync yang asinkron
            Session session = await service.CreateAsync(options);
            return session;
        }
        catch (StripeException e)
        {
            // Log error Stripe dengan detail untuk debugging
            logger.LogError(e, "Stripe API error saat membuat Checkout Session. Stripe Error: {StripeError}", e.StripeError.Message);
            // Anda bisa melempar kembali exception atau mengembalikannya sebagai null tergantung kebutuhan
            return null;
        }
        catch (Exception ex)
        {
            // Menangkap error umum lainnya
            logger.LogError(ex, "Terjadi error tidak terduga saat membuat Stripe Checkout Session.");
            return null;
        }
    }

    // Metode lain dalam service ini bisa ditambahkan di sini...
    // Misalnya, metode yang menggunakan navigationManager atau orderRepository.
}