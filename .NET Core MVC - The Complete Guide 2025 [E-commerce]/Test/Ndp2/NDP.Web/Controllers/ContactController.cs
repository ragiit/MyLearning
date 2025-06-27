// File: /Controllers/ContactController.cs
using Microsoft.AspNetCore.Mvc;
using NDP.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Web; // Untuk HttpUtility

namespace NDP.Controllers
{
    public class ContactController : Controller
    {
        // GET: /Contact/ atau /Contact/Index
        // Action ini akan menampilkan halaman dengan form kosong.
        public IActionResult Index()
        {
            // Jika ada pesan dari pengiriman sebelumnya (menggunakan TempData), teruskan ke View.
            if (TempData["ResultMessage"] != null)
            {
                ViewBag.ResultMessage = TempData["ResultMessage"];
                ViewBag.ResultSuccess = (bool)TempData["ResultSuccess"];
            }

            var model = new ContactFormModel();
            ViewData["Title"] = "Kontak Kami";
            return View(model);
        }

        // POST: /Contact/ atau /Contact/Index
        // Action ini akan dipanggil saat tombol "KIRIM VIA WHATSAPP" di-klik.
        [HttpPost]
        [ValidateAntiForgeryToken] // Untuk keamanan
        public IActionResult Index(ContactFormModel model)
        {
            // Server akan otomatis memvalidasi model berdasarkan atribut [Required], [EmailAddress], dll.
            if (ModelState.IsValid)
            {
                // --- Logika jika form valid ---
                const string whatsAppTargetNumber = "6285156615269"; // Tanpa +

                var waMessage = $"Halo New Design Print,\n\nSaya ingin mengirim pesan dari website:\n------------------------------------\n";
                waMessage += $"Nama: {model.FirstName} {model.LastName}\n";
                waMessage += $"Email: {model.Email}\n";
                waMessage += $"Subjek: {model.Subject}\n\n";
                waMessage += $"Pesan:\n{model.Message}\n------------------------------------\n";
                waMessage += $"Mohon informasinya, terima kasih.";

                var encodedMessage = HttpUtility.UrlEncode(waMessage);
                var whatsappURL = $"https://wa.me/{whatsAppTargetNumber}?text={encodedMessage}";

                // Simpan pesan sukses ke TempData
                TempData["ResultMessage"] = "Form berhasil divalidasi. Anda sedang diarahkan ke WhatsApp...";
                TempData["ResultSuccess"] = true;

                // Alihkan browser pengguna ke URL WhatsApp
                return Redirect(whatsappURL);
            }

            // --- Logika jika form TIDAK valid ---
            // Cukup kembalikan View dengan model yang sama.
            // MVC akan secara otomatis menampilkan pesan error di samping setiap input yang salah.
            ViewData["Title"] = "Kontak Kami";
            return View(model);
        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Nama depan wajib diisi.")]
        [StringLength(50, ErrorMessage = "Nama depan maksimal 50 karakter.")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Nama belakang wajib diisi.")]
        [StringLength(50, ErrorMessage = "Nama belakang maksimal 50 karakter.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Alamat email wajib diisi.")]
        [EmailAddress(ErrorMessage = "Format alamat email tidak valid.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Subjek pesan wajib diisi.")]
        [StringLength(100, ErrorMessage = "Subjek pesan maksimal 100 karakter.")]
        public string Subject { get; set; } = "";

        [Required(ErrorMessage = "Isi pesan wajib diisi.")]
        [StringLength(1000, ErrorMessage = "Isi pesan maksimal 1000 karakter.")]
        public string Message { get; set; } = "";
    }
}