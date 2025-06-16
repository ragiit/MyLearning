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

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = DbSet;
            return query.ToList();
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = DbSet;
            return query.FirstOrDefault(predicate);
        }
    }
}