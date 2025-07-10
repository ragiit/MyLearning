using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyApp.Api.Common;
using MyApp.Service.Dtos;
using MyApp.Service.DTOs;
using MyApp.Service.Interfaces;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookDto>>), 200)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await bookService.GetAllBooksAsync();
            return Ok(ApiResponse<IEnumerable<BookDto>>.Success(books));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(ApiResponse<object>.Fail("Book not found.", 404));
            }
            return Ok(ApiResponse<BookDto>.Success(book));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto bookDto)
        {
            var newBook = await bookService.CreateBookAsync(bookDto);
            if (newBook == null)
            {
                // Kasus ini terjadi jika AuthorId tidak valid
                return BadRequest(ApiResponse<object>.Fail("Invalid AuthorId provided.", 400));
            }

            var response = ApiResponse<BookDto>.Success(newBook, "Book created successfully.", 201);
            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto bookDto)
        {
            var success = await bookService.UpdateBookAsync(id, bookDto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.Fail("Book not found.", 404));
            }
            return Ok(ApiResponse<object>.Success("Book updated successfully."));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await bookService.DeleteBookAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.Fail("Book not found.", 404));
            }
            return Ok(ApiResponse<object>.Success("Book deleted successfully."));
        }
    }
}