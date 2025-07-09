using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Data.Repositories
{
    public class BookRepository(AppDbContext context) : GenericRepository<Book>(context), IBookRepository
    {
    }
}