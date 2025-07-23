namespace Menu.API.Dtos
{
    // Menggunakan PaginationRequest untuk mendukung paginasi
    public record GetCategoriesRequest : PaginationRequest
    {
        [StringLength(100)]
        public string? Name { get; set; } // Filter by category name
        public bool? IsActive { get; set; } // Filter by active status
    }
}