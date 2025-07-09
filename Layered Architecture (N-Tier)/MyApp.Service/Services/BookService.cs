using Mapster;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Service.Dtos;
using MyApp.Service.DTOs;
using MyApp.Service.Interfaces;

namespace MyApp.Service.Services
{
    public class BookService(IUnitOfWork unitOfWork) : IBookService
    {
        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await unitOfWork.Books.GetAllAsync();
            return books.Adapt<IEnumerable<BookDto>>();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await unitOfWork.Books.GetByIdAsync(id);
            return book?.Adapt<BookDto>();
        }

        public async Task<BookDto?> CreateBookAsync(CreateBookDto bookDto)
        {
            // Validasi: Pastikan Author ada sebelum membuat buku
            var authorExists = await unitOfWork.Authors.GetByIdAsync(bookDto.AuthorId);
            if (authorExists == null)
            {
                // Author tidak ditemukan, kembalikan null atau lempar exception
                return null;
            }

            var book = bookDto.Adapt<Book>();
            await unitOfWork.Books.AddAsync(book);
            await unitOfWork.CompleteAsync();

            return book.Adapt<BookDto>();
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto bookDto)
        {
            var existingBook = await unitOfWork.Books.GetByIdAsync(id);
            if (existingBook == null)
            {
                return false; // Buku tidak ditemukan
            }

            // Map properti dari DTO ke entitas yang sudah ada
            bookDto.Adapt(existingBook);

            unitOfWork.Books.Update(existingBook);
            await unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await unitOfWork.Books.GetByIdAsync(id);
            if (book == null)
            {
                return false; // Buku tidak ditemukan
            }

            unitOfWork.Books.Delete(book);
            await unitOfWork.CompleteAsync();
            return true;
        }
    }
}