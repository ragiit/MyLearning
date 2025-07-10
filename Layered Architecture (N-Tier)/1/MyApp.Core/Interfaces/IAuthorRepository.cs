using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        Task<Author?> GetAuthorWithBooksAsync(int authorId);
    }
}