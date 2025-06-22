using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bulky.Models.ViewModels
{
    public class UserVM
    {
        // Data user yang sedang di-edit
        public ApplicationUser ApplicationUser { get; set; }

        // Daftar semua role untuk dropdown
        public IEnumerable<SelectListItem> RoleList { get; set; }

        // Daftar semua company untuk dropdown
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}