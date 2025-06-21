using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        internal DbSet<T> DbSet => context.Set<T>();

        public void Add(T entity)
        {
            context.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public void DeleteAll(IEnumerable<T> values)
        {
            DbSet.RemoveRange(values);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate, string? includes = null)
        {
            IQueryable<T> query = DbSet;
            if (predicate is not null)
                query = query.Where(predicate);
            if (includes != null)
            {
                foreach (var i in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(i);
                }
            }
            return query.ToList();
        }

        public T Get(Expression<Func<T, bool>> predicate, string? includes = null, bool isTracked = false)
        {
            IQueryable<T> query;
            if (isTracked)
            {
                query = DbSet;
            }
            else
            {
                query = DbSet.AsNoTracking();
            }

            if (includes != null)
            {
                foreach (var i in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(i);
                }
            }

            return query.FirstOrDefault(predicate);
        }
    }
}