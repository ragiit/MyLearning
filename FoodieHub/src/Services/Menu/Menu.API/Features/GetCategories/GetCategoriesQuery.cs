namespace Menu.API.Features.GetCategories
{
    // --- QUERY ---
    // Query untuk mendapatkan daftar kategori dengan paginasi dan filter
    public sealed record GetCategoriesQuery([Required] GetCategoriesRequest Request) : IQuery<PaginatedResult<CategoryDto>>;

    // --- VALIDATOR ---
    // Validator untuk GetCategoriesQuery
    public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request data must not be null.");

            // Validasi untuk properti paginasi (dari PaginationRequest)
            RuleFor(x => x.Request.PageIndex)
                .GreaterThanOrEqualTo(0).WithMessage("PageIndex must be at least 0.");
            RuleFor(x => x.Request.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
        }
    }

    // --- HANDLER ---
    // Handler yang memproses GetCategoriesQuery
    public class GetCategoriesHandler(ApplicationDbContext db) : IQueryHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>
    {
        public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = db.Categories.AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.Request.Name))
            {
                query = query.Where(c => c.Name.Contains(request.Request.Name));
            }

            if (request.Request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.Request.IsActive.Value);
            }

            // Get total count before pagination
            var totalCount = await query.LongCountAsync(cancellationToken);

            // Pagination logic
            var categories = await query
                .OrderBy(c => c.Name) // Order by for consistent pagination
                .Skip(request.Request.PageIndex * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var categoryDtos = categories.Adapt<List<CategoryDto>>(); // Menggunakan Mapster untuk mapping

            // Return PaginatedResult
            return new PaginatedResult<CategoryDto>(
                request.Request.PageIndex,
                request.Request.PageSize,
                totalCount,
                categoryDtos
            );
        }
    }
}