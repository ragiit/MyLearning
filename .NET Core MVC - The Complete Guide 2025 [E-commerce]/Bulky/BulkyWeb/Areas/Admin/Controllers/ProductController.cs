using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // DIUBAH: Tambahkan IWebHostEnvironment untuk mengakses path wwwroot saat upload gambar.
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // FIX: Menambahkan includeProperties yang sebelumnya terlewat
            List<Product> productList = [.. _unitOfWork.Product.GetAll(includes: "Category")];
            return View(productList);
        }

        // GET: UNTUK CREATE DAN EDIT
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                // Mode CREATE: Cukup tampilkan view dengan product baru yang kosong
                return View(productVM);
            }
            else
            {
                // Mode EDIT: Ambil produk dari DB dan kirim ke view
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        // POST: UNTUK CREATE DAN EDIT
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // --- LOGIKA UPLOAD FILE GAMBAR ---
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    // Jika sedang mengedit dan ada gambar lama, hapus gambar lama.
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Simpan file baru
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                // --- AKHIR LOGIKA UPLOAD FILE ---

                // Tentukan apakah ini Add (Create) atau Update (Edit)
                if (productVM.Product.Id == 0)
                {
                    // Mode CREATE
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Produk berhasil dibuat.";
                }
                else
                {
                    // Mode EDIT
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Produk berhasil diperbarui.";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else // Jika Model State tidak valid
            {
                // Isi kembali dropdown kategori sebelum mengirim view kembali.
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = [.. _unitOfWork.Product.GetAll(includes: "Category")];

            return Json(new
            {
                data = productList
            });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // PERBAIKAN UTAMA ADA DI SINI
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                // Kembalikan JSON dengan pesan error
                return Json(new { success = false, message = "Gagal: Data tidak ditemukan." });
            }

            // Hapus gambar terkait jika ada
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();

            // Kembalikan JSON dengan pesan sukses
            return Json(new { success = true, message = "Produk berhasil dihapus." });
        }

        #endregion API CALLS
    }
}