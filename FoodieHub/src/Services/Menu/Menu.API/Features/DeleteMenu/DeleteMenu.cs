using Menu.API.Services.IService;

namespace Menu.API.Features.DeleteMenu
{
    // --- COMMAND ---
    // Command untuk menghapus menu, tidak perlu request DTO terpisah jika hanya butuh ID
    public sealed record DeleteMenuCommand(Guid Id) : ICommand<Unit>; // Mengembalikan MediatR.Unit

    // --- VALIDATOR ---
    public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
    {
        private readonly ApplicationDbContext _db;

        public DeleteMenuCommandValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Menu ID is required.")
                .MustAsync(MenuMustExist).WithMessage("Menu with this ID does not exist.");
        }

        // Custom validation method for menu existence
        private async Task<bool> MenuMustExist(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Menus.AnyAsync(m => m.Id == id, cancellationToken);
        }
    }

    // --- HANDLER ---
    public class DeleteMenuHandler(ApplicationDbContext db, IFileStorageService fileStorageService) : ICommandHandler<DeleteMenuCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus
                .Include(x => x.Images)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
                ?? throw new MenuNotFoundException(request.Id);

            db.Menus.Remove(menu);

            if (!string.IsNullOrEmpty(menu.ImageUrl))
            {
                var fileName = Path.GetFileName(menu.ImageUrl);
                await fileStorageService.DeleteFileAsync(fileName, "menu-images");
            }

            if (menu.Images != null && menu.Images.Count != 0)
            {
                foreach (var image in menu.Images)
                {
                    var fileName = Path.GetFileName(image.Url);
                    await fileStorageService.DeleteFileAsync(fileName, "menu-images/additional");
                }
                db.MenuImages.RemoveRange(menu.Images);
            }

            await db.SaveChangesAsync(cancellationToken);

            return Unit.Value; // Mengembalikan Unit.Value untuk command yang tidak memiliki return value
        }
    }
}