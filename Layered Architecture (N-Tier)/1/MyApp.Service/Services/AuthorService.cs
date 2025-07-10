using Mapster;
using MyApp.Core.Entities;
using MyApp.Core.Interfaces;
using MyApp.Service.Dtos;
using MyApp.Service.Interfaces;

namespace MyApp.Service.Services
{
    public class AuthorService(IUnitOfWork unitOfWork) : IAuthorService
    {
        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await unitOfWork.Authors.GetAllAsync();
            return authors.Adapt<IEnumerable<AuthorDto>>();
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            // Menggunakan method khusus dari AuthorRepository
            var author = await unitOfWork.Authors.GetAuthorWithBooksAsync(id);
            return author?.Adapt<AuthorDto>();
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto authorDto)
        {
            var author = authorDto.Adapt<Author>();
            await unitOfWork.Authors.AddAsync(author);
            await unitOfWork.CompleteAsync();
            return author.Adapt<AuthorDto>();
        }
    }
}