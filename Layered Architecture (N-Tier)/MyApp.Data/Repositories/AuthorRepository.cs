using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;

namespace MyApp.Data.Repositories
{
    public class AuthorRepository(AppDbContext context) : GenericRepository<Author>(context), IAuthorRepository
    {
        public async Task<Author?> GetAuthorWithBooksAsync(int authorId)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == authorId);
        }
    }
}