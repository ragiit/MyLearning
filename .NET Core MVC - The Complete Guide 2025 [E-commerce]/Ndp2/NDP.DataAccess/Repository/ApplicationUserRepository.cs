using Microsoft.Extensions.Caching.Memory;
using NDP.DataAccess.Data;
using NDP.DataAccess.Repository.IRepository;
using NDP.Models.Models;

namespace NDP.DataAccess.Repository
{
    public class ApplicationUserRepository(ApplicationDbContext context, IMemoryCache cache) : Repository<ApplicationUser>(context, cache), IApplicationUserRepository
    {
    }
}