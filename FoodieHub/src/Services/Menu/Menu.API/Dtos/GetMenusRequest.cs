// Menu.API/Dtos/GetMenusRequest.cs

namespace Menu.API.Dtos
{
    public record GetMenusRequest : PaginationRequest
    {
        [StringLength(100)]
        public string? CategoryName { get; set; } // Filter by category name

        [StringLength(250)]
        public string? SearchTerm { get; set; } // Search by menu name/description

        [Range(0, 10000)] // Example range
        public decimal? MinPrice { get; set; }

        [Range(0, 10000)]
        public decimal? MaxPrice { get; set; }

        public bool? IsAvailable { get; set; }
    }
}