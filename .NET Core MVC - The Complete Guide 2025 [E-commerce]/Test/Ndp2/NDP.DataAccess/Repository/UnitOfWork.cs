using Microsoft.Extensions.Caching.Memory;
using NDP.DataAccess.Data;
using NDP.DataAccess.Repository.IRepository;

namespace NDP.DataAccess.Repository
{
    public class UnitOfWork(ApplicationDbContext context, IMemoryCache cache) : IUnitOfWork
    {
        private IApplicationUserRepository? _userRepository;

        public IApplicationUserRepository UserRepository
        {
            get
            {
                _userRepository ??= new ApplicationUserRepository(context, cache);
                return _userRepository;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose() => context.Dispose();
    }
}