// Menu.API/Features/GetMenus/GetMenus.cs

namespace Menu.API.Features.GetMenuQueries
{
    public sealed record GetMenusQuery([Required] GetMenusRequest Request) : IQuery<PaginatedResult<MenuDto>>;

    public class GetMenusQueryValidator : AbstractValidator<GetMenusQuery>
    {
        public GetMenusQueryValidator()
        {
            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request data must not be null.");

            RuleFor(x => x.Request.PageIndex)
                .GreaterThanOrEqualTo(0).WithMessage("PageIndex must be at least 0.");
            RuleFor(x => x.Request.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            // Validasi untuk filter harga
            RuleFor(x => x.Request.MaxPrice)
                .GreaterThanOrEqualTo(x => x.Request.MinPrice)
                .When(x => x.Request.MinPrice.HasValue && x.Request.MaxPrice.HasValue)
                .WithMessage("MaxPrice cannot be less than MinPrice.");
        }
    }

    // --- HANDLER ---
    // Handler yang memproses GetMenusQuery
    public class GetMenusHandler(ApplicationDbContext db) : IQueryHandler<GetMenusQuery, PaginatedResult<MenuDto>>
    {
        public async Task<PaginatedResult<MenuDto>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
        {
            var query = db.Menus.Include(m => m.Category).AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.Request.CategoryName))
            {
                query = query.Where(m => m.Category.Name.Contains(request.Request.CategoryName));
            }

            if (!string.IsNullOrWhiteSpace(request.Request.SearchTerm))
            {
                query = query.Where(m => m.Name.Contains(request.Request.SearchTerm) ||
                                         m.Description != null && m.Description.Contains(request.Request.SearchTerm));
            }

            if (request.Request.MinPrice.HasValue)
            {
                query = query.Where(m => m.Price >= request.Request.MinPrice.Value);
            }

            if (request.Request.MaxPrice.HasValue)
            {
                query = query.Where(m => m.Price <= request.Request.MaxPrice.Value);
            }

            if (request.Request.IsAvailable.HasValue)
            {
                query = query.Where(m => m.IsAvailable == request.Request.IsAvailable.Value);
            }

            // Dapatkan total count sebelum paginasi
            var totalCount = await query.LongCountAsync(cancellationToken);

            // Pagination logic
            var menus = await query
                .OrderBy(m => m.Name) // Penting untuk konsistensi paginasi
                .Skip(request.Request.PageIndex * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var menuDtos = menus.Adapt<List<MenuDto>>();

            // Kembalikan PaginatedResult
            return new PaginatedResult<MenuDto>(
                request.Request.PageIndex,
                request.Request.PageSize,
                totalCount,
                menuDtos
            );
        }
    }
}