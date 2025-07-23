namespace Menu.API.Features.CreateMenu
{
    // --- COMMAND ---
    public sealed record CreateMenuCommand(CreateMenuRequest Request) : ICommand<MenuDto>;

    // --- VALIDATOR ---
    public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
    {
        private readonly ApplicationDbContext _db;

        public CreateMenuCommandValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request data must not be null.");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 250).WithMessage("Name must be between 1 and 250 characters.")
                .MustAsync(BeUniqueName).WithMessage("Menu with this name already exists.");

            RuleFor(x => x.Request.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Request.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.")
                .MustAsync(CategoryMustExist).WithMessage("Category does not exist.");
        }

        // Custom validation method for unique menu name
        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            return await _db.Menus.AllAsync(m => m.Name != name, cancellationToken);
        }

        // Custom validation method to check if category exists
        private async Task<bool> CategoryMustExist(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _db.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
        }
    }

    // --- HANDLER ---
    public class CreateMenuHandler(ApplicationDbContext db) : ICommandHandler<CreateMenuCommand, MenuDto>
    {
        public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            // Map request to Menu entity
            var menu = request.Request.Adapt<Menu.API.Persistence.Entities.Menu>();

            // Set audit fields (jika ada user info dari token JWT)
            // Anda bisa mendapatkan User ID dari HttpContextAccessor atau melalui parameter constructor handler
            // Untuk kesederhanaan, kita asumsikan ID user di sini atau dari claim JWT
            // string userId = "System"; // Placeholder, ganti dengan User ID dari token
            // menu.CreatedBy = userId;
            menu.CreatedDate = DateTime.UtcNow;

            db.Menus.Add(menu);
            await db.SaveChangesAsync(cancellationToken);

            // Map created Menu entity back to MenuDto
            return menu.Adapt<MenuDto>();
        }
    }
}