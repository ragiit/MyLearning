using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController(IUnitOfWork unitOfWork) : Controller
    {
        // Action untuk menampilkan halaman daftar (yang akan menggunakan DataTables)
        public IActionResult Index()
        {
            return View();
        }

        // GET Action untuk form Upsert (Create dan Edit)
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Mode CREATE: Kirim objek Company baru yang kosong
                return View(new Company());
            }
            else
            {
                // Mode EDIT: Ambil data Company dari database
                Company? companyFromDb = unitOfWork.Company.Get(u => u.Id == id);
                if (companyFromDb == null)
                {
                    return NotFound();
                }
                return View(companyFromDb);
            }
        }

        // POST Action untuk form Upsert
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    // Create
                    unitOfWork.Company.Add(company);
                    TempData["success"] = "Perusahaan berhasil dibuat.";
                }
                else
                {
                    // Update
                    unitOfWork.Company.Update(company);
                    TempData["success"] = "Perusahaan berhasil diperbarui.";
                }

                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            // Jika model tidak valid, kembali ke form dengan data yang sudah diisi
            return View(company);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyList = unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Gagal: Data tidak ditemukan." });
            }

            unitOfWork.Company.Delete(companyToBeDeleted);
            unitOfWork.Save();

            return Json(new { success = true, message = "Data berhasil dihapus." });
        }

        #endregion API CALLS
    }
}