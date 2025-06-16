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

        // DIUBAH: Menggunakan Dependency Injection standar dan menyimpan IUnitOfWork di field private.
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // PENJELASAN: Menggunakan includeProperties untuk mengambil data Category terkait (Eager Loading).
            // Ini memungkinkan Anda menampilkan nama kategori di halaman daftar produk tanpa query tambahan.
            List<Product> productList = _unitOfWork.Product.GetAll().ToList();

            // Variabel 'c' tidak digunakan dan dihapus. Data yang dikirim ke View sudah benar.
            return View(productList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Kode ini sudah benar.
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            // Custom validation bisa diletakkan di sini jika perlu.
            if (productVM.Product.Title != null &&
                productVM.Product.Title.Equals("test", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("Product.Title", "The name cannot be 'test'.");
            }

            if (ModelState.IsValid)
            {
                // DIUBAH: Mengambil objek Product dari dalam ViewModel.
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();

                TempData["success"] = $"Product '{productVM.Product.Title}' was created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                // KRUSIAL: Jika validasi gagal, dropdown kategori harus diisi kembali sebelum dikirim ke View.
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // DIUBAH: Menggunakan ProductVM agar View Edit bisa menampilkan dropdown kategori.
            ProductVM productVM = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == id),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            if (productVM.Product == null)
            {
                return NotFound();
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();

                TempData["success"] = $"Product '{productVM.Product.Title}' was updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                // KRUSIAL: Isi kembali dropdown jika validasi gagal.
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        // DIUBAH: Metode Delete dipecah menjadi dua: GET untuk menampilkan halaman konfirmasi, POST untuk eksekusi.

        // GET - DELETE (Menampilkan halaman konfirmasi)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Menggunakan eager loading jika perlu menampilkan nama kategori di halaman delete.
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        // POST - DELETE (Eksekusi hapus data)
        [HttpGet]
        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Delete(obj);
            _unitOfWork.Save();

            TempData["success"] = $"Product '{obj.Title}' was deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}