using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Helper.Role_Admin)]
    public class UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET - HALAMAN MANAJEMEN PERAN
        public IActionResult RoleManagement(string userId)
        {
            // Ambil role pengguna saat ini
            string oldRole = db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            UserVM userVM = new()
            {
                ApplicationUser = db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                // Ambil semua role dan company untuk dropdown
                RoleList = db.Roles.Select(i => new SelectListItem { Text = i.Name, Value = i.Name }),
                CompanyList = db.Companies.Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
            };

            // Set role pengguna saat ini di properti [NotMapped]
            userVM.ApplicationUser.Role = db.Roles.FirstOrDefault(u => u.Id == oldRole).Name;
            return View(userVM);
        }

        // POST - PROSES PERUBAHAN PERAN
        [HttpPost]
        public IActionResult RoleManagement(UserVM userVM)
        {
            string oldRole = db.UserRoles.FirstOrDefault(u => u.UserId == userVM.ApplicationUser.Id).RoleId;
            string oldRoleName = db.Roles.FirstOrDefault(u => u.Id == oldRole).Name;

            // Cek jika ada perubahan role
            if (userVM.ApplicationUser.Role != oldRoleName)
            {
                ApplicationUser applicationUser = db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Id);

                // Jika role baru adalah Company, set CompanyId
                if (userVM.ApplicationUser.Role == Helper.Role_Company)
                {
                    applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
                }
                // Jika role lama adalah Company, hapus CompanyId
                if (oldRoleName == Helper.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                db.SaveChanges();

                // Hapus role lama dan tambahkan role baru
                userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser, userVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            // Jika role tidak berubah tapi role nya adalah Company, mungkin ada perubahan company
            else if (oldRoleName == Helper.Role_Company && userVM.ApplicationUser.CompanyId != userVM.ApplicationUser.CompanyId)
            {
                ApplicationUser applicationUser = db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Id);
                applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
                db.SaveChanges();
            }

            TempData["success"] = "Peran pengguna berhasil diperbarui.";
            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Ambil semua user dari database
            List<ApplicationUser> userList = db.ApplicationUsers.Include(u => u.Company).ToList();

            // Ambil semua role dan user-role mapping
            var userRoles = db.UserRoles.ToList();
            var roles = db.Roles.ToList();

            // Loop melalui setiap user untuk menambahkan informasi role mereka
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId)?.Name;

                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var userFromDb = db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error: Pengguna tidak ditemukan." });
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                // Pengguna saat ini terkunci, kita akan membukanya
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Pengguna tidak terkunci, kita akan menguncinya
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            db.SaveChanges();
            return Json(new { success = true, message = "Operasi berhasil." });
        }

        #endregion API CALLS
    }
}