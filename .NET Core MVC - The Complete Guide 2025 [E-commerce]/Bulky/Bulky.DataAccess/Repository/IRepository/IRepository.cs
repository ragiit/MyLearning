using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includes = null);

        T Get(Expression<Func<T, bool>> predicate, string? includes = null);

        void Add(T entity);

        void Delete(T entity);

        void DeleteAll(IEnumerable<T> values);
    }
}