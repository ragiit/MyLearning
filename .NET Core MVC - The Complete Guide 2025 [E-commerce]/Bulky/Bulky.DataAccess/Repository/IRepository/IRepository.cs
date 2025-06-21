using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includes = null);

        T Get(Expression<Func<T, bool>> predicate, string? includes = null, bool isTracked = false);

        void Add(T entity);

        void Delete(T entity);

        void DeleteAll(IEnumerable<T> values);
    }
}