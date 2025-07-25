using Menu.API.Features.CreateMenu;
using Menu.API.Features.DeleteMenu;
using Menu.API.Features.GetCategories;
using Menu.API.Features.MenuImages;
using Menu.API.Features.UpdateMenu;

namespace Menu.API.Controllers
{
    /// <summary>
    /// Controller untuk operasi terkait menu dan kategori makanan.
    /// </summary>
    [Route("api/menus")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public class MenuController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Mendapatkan daftar menu dengan opsi filter dan paginasi.
        /// </summary>
        /// <remarks>
        /// Mengambil daftar item menu dari database. Mendukung filter berdasarkan nama kategori, kata kunci pencarian,
        /// rentang harga, dan ketersediaan. Hasil dapat dipaginasi.
        /// </remarks>
        /// <param name="request">Parameter filter dan paginasi untuk daftar menu.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Daftar menu yang difilter dan dipaginasi.</returns>
        /// <response code="200">Daftar menu berhasil diambil.</response>
        /// <response code="400">Request tidak valid atau terdapat kesalahan validasi.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<PaginatedResult<MenuDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<PaginatedResult<MenuDto>>>> GetMenus([FromQuery] GetMenusRequest request, CancellationToken cancellationToken)
        {
            var query = new GetMenusQuery(request);
            var result = await sender.Send(query, cancellationToken);

            var response = new BaseResponse<PaginatedResult<MenuDto>>(true, "Menus retrieved successfully", result);
            return Ok(response);
        }

        /// <summary>
        /// Membuat item menu baru.
        /// </summary>
        /// <remarks>
        /// Menambahkan item menu baru ke database dengan detail yang diberikan.
        /// Nama menu harus unik dan CategoryId harus valid.
        /// </remarks>
        /// <param name="request">Data untuk membuat menu baru.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Detail menu yang baru dibuat.</returns>
        /// <response code="201">Menu berhasil dibuat.</response>
        /// <response code="400">Request tidak valid, kesalahan validasi, atau nama menu sudah ada.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<MenuDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // Untuk unique name jika diimplementasikan
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<MenuDto>>> CreateMenu([FromForm] CreateMenuRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateMenuCommand(request);
            var result = await sender.Send(command, cancellationToken);

            var response = new BaseResponse<MenuDto>(true, "Menu created successfully", result);
            // Gunakan CreatedAtAction untuk respons 201 Created
            return CreatedAtAction(nameof(GetMenus), new { id = result.Id }, response); // Asumsi ada GetMenuById endpoint
        }

        /// <summary>
        /// Memperbarui detail item menu yang sudah ada.
        /// </summary>
        /// <remarks>
        /// Memperbarui detail item menu berdasarkan ID yang diberikan.
        /// Nama menu harus unik di antara menu lain (kecuali menu itu sendiri).
        /// </remarks>
        /// <param name="request">Data untuk memperbarui menu.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Detail menu yang telah diperbarui.</returns>
        /// <response code="200">Menu berhasil diperbarui.</response>
        /// <response code="400">Request tidak valid, kesalahan validasi, atau nama menu duplikat.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Menu tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpPut]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<MenuDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // Untuk unique name jika diimplementasikan
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<MenuDto>>> UpdateMenu([FromForm] UpdateMenuRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateMenuCommand(request);
            var result = await sender.Send(command, cancellationToken);

            var response = new BaseResponse<MenuDto>(true, "Menu updated successfully", result);
            return Ok(response);
        }

        /// <summary>
        /// Menghapus item menu berdasarkan ID.
        /// </summary>
        /// <param name="id">ID unik dari menu yang akan dihapus.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Respons kosong jika penghapusan berhasil.</returns>
        /// <response code="204">Menu berhasil dihapus (No Content).</response>
        /// <response code="400">Request tidak valid atau ID menu kosong.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Menu tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpDelete("{id}")] // DELETE /api/menus/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMenu(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteMenuCommand(id);
            await sender.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Mendapatkan daftar kategori menu dengan opsi filter dan paginasi.
        /// </summary>
        /// <remarks>
        /// Mengambil daftar kategori menu dari database. Mendukung filter berdasarkan nama kategori dan status aktif.
        /// Hasil dapat dipaginasi.
        /// </remarks>
        /// <param name="request">Parameter filter dan paginasi untuk daftar kategori.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Daftar kategori yang difilter dan dipaginasi.</returns>
        /// <response code="200">Daftar kategori berhasil diambil.</response>
        /// <response code="400">Request tidak valid atau terdapat kesalahan validasi.</response>
        /// <response code="401">Akses tidak sah jika token JWT tidak valid atau tidak tersedia.</response>
        /// <response code="429">Terlalu banyak permintaan (Rate Limit Exceeded).</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpGet("categories")] // Endpoint untuk kategori
        [ProducesResponseType(typeof(BaseResponse<PaginatedResult<CategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<PaginatedResult<CategoryDto>>>> GetCategories([FromQuery] GetCategoriesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetCategoriesQuery(request);
            var result = await sender.Send(query, cancellationToken);

            var response = new BaseResponse<PaginatedResult<CategoryDto>>(true, "Categories retrieved successfully", result);
            return Ok(response);
        }

        /// <summary>
        /// Menambahkan gambar baru ke item menu tertentu.
        /// </summary>
        /// <remarks>
        /// Mengupload gambar baru dan mengaitkannya dengan item menu.
        /// Bisa ditandai sebagai gambar thumbnail utama, yang akan menggantikan thumbnail sebelumnya.
        /// </remarks>
        /// <param name="menuId">ID unik dari menu.</param>
        /// <param name="imageFile">File gambar yang akan diupload.</param>
        /// <param name="isThumbnail">Menentukan apakah gambar ini akan menjadi thumbnail utama.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Detail gambar yang baru ditambahkan.</returns>
        /// <response code="201">Gambar berhasil ditambahkan.</response>
        /// <response code="400">Request tidak valid, kesalahan validasi (misal, format file salah).</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Menu tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpPost("{menuId}/images")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<MenuImageDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<MenuImageDto>>> AddMenuImage(
            Guid menuId,
            IFormFile request,
            CancellationToken cancellationToken)
        {
            var command = new AddMenuImageCommand(new AddMenuImageRequest(menuId, request));
            var result = await sender.Send(command, cancellationToken);

            var response = new BaseResponse<MenuImageDto>(true, "Menu image added successfully", result);
            return CreatedAtAction(nameof(AddMenuImage), new { menuId = result.MenuId, imageId = result.Id }, response);
        }

        /// <summary>
        /// Menghapus gambar dari item menu.
        /// </summary>
        /// <remarks>
        /// Menghapus gambar tertentu dari item menu dan storage.
        /// Jika gambar yang dihapus adalah thumbnail utama, ImageUrl di menu akan di-reset.
        /// </remarks>
        /// <param name="menuId">ID unik dari menu.</param>
        /// <param name="imageId">ID unik dari gambar yang akan dihapus.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Respons kosong jika penghapusan berhasil.</returns>
        /// <response code="204">Gambar berhasil dihapus (No Content).</response>
        /// <response code="400">Request tidak valid.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Gambar tidak ditemukan atau tidak terkait dengan menu.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpDelete("{menuId}/images/{url}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteMenuImage(Guid menuId, string url, CancellationToken cancellationToken)
        {
            var command = new DeleteMenuImageCommand(menuId, url);
            await sender.Send(command, cancellationToken);

            return NoContent();
        }
    }
}