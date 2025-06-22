using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent(IUnitOfWork unitOfWork) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Ambil user yang sedang login
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) // Jika user sudah login
            {
                // Cek apakah session sudah ada
                if (HttpContext.Session.GetInt32(Helper.SessionCart) == null)
                {
                    // Jika belum, query ke DB dan buat session
                    var cartCount = unitOfWork.Cart
                        .GetAll(u => u.ApplicationUserId == claim.Value)
                        .Count();
                    HttpContext.Session.SetInt32(Helper.SessionCart, cartCount);
                    return View(cartCount);
                }
                else
                {
                    // Jika sudah ada, langsung ambil dari session
                    return View(HttpContext.Session.GetInt32(Helper.SessionCart));
                }
            }
            else
            {
                // Jika user belum login, hapus session dan tampilkan 0
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}