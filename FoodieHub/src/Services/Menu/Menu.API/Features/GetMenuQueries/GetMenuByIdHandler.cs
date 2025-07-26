namespace Menu.API.Features.GetMenuQueries
{
    // --- QUERY ---
    // Query untuk mendapatkan detail menu berdasarkan ID
    public sealed record GetMenuByIdQuery(Guid Id) : IQuery<MenuDto>;

    // --- VALIDATOR ---
    // Validator untuk GetMenuByIdQuery
    public class GetMenuByIdQueryValidator : AbstractValidator<GetMenuByIdQuery>
    {
        public GetMenuByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Menu ID tidak boleh kosong.");
        }
    }

    // --- HANDLER ---
    // Handler yang memproses GetMenuByIdQuery
    public class GetMenuByIdHandler(ApplicationDbContext db) : IQueryHandler<GetMenuByIdQuery, MenuDto>
    {
        public async Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
        {
            // Mencari menu berdasarkan ID dan meload data kategori terkait
            var menu = await db.Menus
                .Include(m => m.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            // Jika menu tidak ditemukan, lempar exception
            if (menu == null)
            {
                throw new MenuNotFoundException(request.Id);
            }

            // Map entitas Menu ke MenuDto
            return menu.Adapt<MenuDto>();
        }
    }
}