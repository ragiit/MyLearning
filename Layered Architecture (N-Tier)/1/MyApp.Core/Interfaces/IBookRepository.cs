using MyApp.Core.Entities;

namespace MyApp.Core.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        // Tambahkan method khusus untuk Book jika diperlukan di masa depan
        // Contoh: Task<IEnumerable<Book>> GetPopularBooksAsync();
    }
}