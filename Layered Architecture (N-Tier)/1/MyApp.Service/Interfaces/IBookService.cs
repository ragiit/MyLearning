using MyApp.Service.Dtos;
using MyApp.Service.DTOs;

namespace MyApp.Service.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();

        Task<BookDto?> GetBookByIdAsync(int id);

        Task<BookDto?> CreateBookAsync(CreateBookDto bookDto);

        Task<bool> UpdateBookAsync(int id, UpdateBookDto bookDto);

        Task<bool> DeleteBookAsync(int id);
    }
}