using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository(ApplicationDbContext context) : Repository<ApplicationUser>(context), IApplicationUserRepository
    {
        //public void Update(ApplicationUser ApplicationUser)
        //{
        //    context.Categories.Update(ApplicationUser);
        //}
    }
}