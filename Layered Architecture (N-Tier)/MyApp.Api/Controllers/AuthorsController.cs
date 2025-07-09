using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Common;
using MyApp.Service.Dtos;
using MyApp.Service.Interfaces;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController(IAuthorService authorService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AuthorDto>>), 200)]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await authorService.GetAllAuthorsAsync();

            // Bungkus data dengan ApiResponse
            var response = ApiResponse<IEnumerable<AuthorDto>>.Success(authors);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                // Gunakan factory method Fail
                var response = ApiResponse<object>.Fail("Author not found.", 404);
                return NotFound(response);
            }

            // Gunakan factory method Success
            var successResponse = ApiResponse<AuthorDto>.Success(author);
            return Ok(successResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuthorDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            if (!ModelState.IsValid)
            {
                // Ambil daftar error dari ModelState
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                var response = ApiResponse<object>.Fail("Invalid input provided.", 400, errors);
                return BadRequest(response);
            }

            var newAuthor = await authorService.CreateAuthorAsync(createAuthorDto);

            var successResponse = ApiResponse<AuthorDto>.Success(newAuthor, "Author created successfully.", 201);

            // Mengembalikan status 201 Created dengan lokasi resource baru
            return CreatedAtAction(nameof(GetAuthorById), new { id = newAuthor.Id }, successResponse);
        }
    }
}