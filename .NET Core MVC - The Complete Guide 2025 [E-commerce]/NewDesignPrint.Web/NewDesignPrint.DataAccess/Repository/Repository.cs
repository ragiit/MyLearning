using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NDP.DataAccess.Data;
using NDP.DataAccess.Repository.IRepository;
using System.Linq.Expressions;

namespace NDP.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        internal DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null, bool isTracked = false, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"{typeof(T).Name}-{predicate}-{includeProperties ?? "none"}";

            // Coba ambil dari cache terlebih dahulu
            if (_cache.TryGetValue(cacheKey, out T? cachedResult))
            {
                return cachedResult;
            }

            IQueryable<T> query = isTracked ? _dbSet : _dbSet.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split([','], StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            var result = await query.FirstOrDefaultAsync(predicate, cancellationToken);

            if (result != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, result, cacheEntryOptions);
            }

            return result;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}