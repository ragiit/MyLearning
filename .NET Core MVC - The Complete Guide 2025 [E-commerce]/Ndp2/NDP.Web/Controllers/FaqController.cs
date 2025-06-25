using Microsoft.AspNetCore.Mvc;

namespace NDP.Web.Controllers
{
    public class FaqController : Controller
    {
        public class FaqItemViewModel
        {
            public int Id { get; set; }
            public string Question { get; set; } = string.Empty;
            public string Answer { get; set; } = string.Empty; // Bisa berisi HTML
        }

        public class FaqService
        {
            public Task<List<FaqItemViewModel>> GetFaqItemsAsync()
            {
                var faqItems = new List<FaqItemViewModel>
            {
                new() {
                    Id = 1,
                    Question = "Bagaimana cara memesan produk custom di New Design Print?",
                    Answer = @"<p>Anda bisa memesan dengan mudah melalui beberapa langkah:</p>
                              <ol class='list-decimal list-inside ml-4 space-y-1'>
                                <li><strong>Konsultasi:</strong> Hubungi kami via WhatsApp atau email untuk membahas kebutuhan Anda (produk, jumlah, desain).</li>
                                <li><strong>Penawaran & Desain:</strong> Kami akan memberikan penawaran. Setelah setuju, kami akan proses desain (bisa dari Anda atau kami bantu).</li>
                                <li><strong>Pembayaran:</strong> Lakukan pembayaran sesuai kesepakatan (misalnya DP).</li>
                                <li><strong>Produksi:</strong> Produk Anda akan kami proses setelah pembayaran dikonfirmasi.</li>
                                <li><strong>Pengiriman/Pengambilan:</strong> Kami akan informasikan jika produk sudah selesai dan siap dikirim atau diambil.</li>
                              </ol>
                              <p class='mt-2'>Untuk detail lebih lanjut, silakan kunjungi halaman <a href='/how-to-order' class='text-pink-600 hover:underline font-semibold'>Cara Pesan</a> kami.</p>"
                },
                new FaqItemViewModel {
                    Id = 2,
                    Question = "Berapa lama proses produksi biasanya?",
                    Answer = "<p>Lama proses produksi bervariasi tergantung jenis produk, jumlah pesanan, dan tingkat kerumitan desain. Rata-rata untuk produk seperti kaos atau mug bisa memakan waktu 3-7 hari kerja setelah desain dan pembayaran disetujui. Untuk pesanan dalam jumlah besar atau produk yang lebih kompleks, waktunya mungkin lebih lama. Kami akan selalu memberikan estimasi waktu pengerjaan saat Anda melakukan pemesanan.</p>"
                },
                // ... Tambahkan item FAQ lainnya dari kode Blazor Anda ...
            };
                return Task.FromResult(faqItems);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}