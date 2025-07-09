using MyApp.Service.Dtos;

namespace MyApp.Service.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorDto?> GetAuthorByIdAsync(int id);

        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();

        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto authorDto);
    }
}