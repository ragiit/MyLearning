using Menu.API.Services.IService;

namespace Menu.API.Features.UpdateMenu
{
    // --- COMMAND ---
    public sealed record UpdateMenuCommand(UpdateMenuRequest Request) : ICommand<MenuDto>;

    // --- VALIDATOR ---
    public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
    {
        private readonly ApplicationDbContext _db;

        public UpdateMenuCommandValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request data must not be null.");

            RuleFor(x => x.Request.Id)
                .NotEmpty().WithMessage("Menu ID is required for update.")
                .MustAsync(MenuMustExist).WithMessage("Menu with this ID does not exist.");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 250).WithMessage("Name must be between 1 and 250 characters.")
                .MustAsync(BeUniqueNameWhenUpdating).WithMessage("Another menu with this name already exists.");

            RuleFor(x => x.Request.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Request.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.")
                .MustAsync(CategoryMustExist).WithMessage("Category does not exist.");
        }

        // Custom validation method for menu existence
        private async Task<bool> MenuMustExist(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Menus.AnyAsync(m => m.Id == id, cancellationToken);
        }

        // Custom validation method for unique menu name when updating
        private async Task<bool> BeUniqueNameWhenUpdating(UpdateMenuCommand command, string name, CancellationToken cancellationToken)
        {
            return await _db.Menus.AllAsync(m => m.Name != name || m.Id == command.Request.Id, cancellationToken);
        }

        // Custom validation method to check if category exists
        private async Task<bool> CategoryMustExist(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _db.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
        }
    }

    // --- HANDLER ---
    public class UpdateMenuHandler(ApplicationDbContext db, IFileStorageService fileStorageService) : ICommandHandler<UpdateMenuCommand, MenuDto>
    {
        public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus
                .FirstOrDefaultAsync(m => m.Id == request.Request.Id, cancellationToken)
                ?? throw new MenuNotFoundException(request.Request.Id);

            var oldImageUrl = menu.ImageUrl;

            request.Request.Adapt(menu);

            if (request.Request.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    var oldFileName = Path.GetFileName(oldImageUrl);
                    await fileStorageService.DeleteFileAsync(oldFileName, "menu-images");
                }

                menu.ImageUrl = await fileStorageService.UploadFileAsync(request.Request.ImageUrl, "menu-images");
            }

            if (request.Request.AdditionalImages != null && request.Request.AdditionalImages.Count != 0)
            {
                foreach (var file in request.Request.AdditionalImages)
                {
                    var additionalImageUrl = await fileStorageService.UploadFileAsync(file, "menu-images/additional");
                    db.MenuImages.Add(new MenuImage { MenuId = menu.Id, Url = additionalImageUrl, IsThumbnail = false });
                }
                await db.SaveChangesAsync(cancellationToken);
            }

            menu.LastModifiedDate = DateTime.UtcNow;

            db.Menus.Update(menu);
            await db.SaveChangesAsync(cancellationToken);

            var updatedMenuWithCategory = await db.Menus
                .Include(m => m.Category)
                 .Include(m => m.Images)
                .Where(m => m.Id == menu.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (updatedMenuWithCategory == null)
            {
                throw new InvalidOperationException("Failed to retrieve the updated menu with its category.");
            }

            return updatedMenuWithCategory.Adapt<MenuDto>();
        }
    }
}