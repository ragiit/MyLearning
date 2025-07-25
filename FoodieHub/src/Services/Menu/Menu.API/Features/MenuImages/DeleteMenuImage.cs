// Menu.API/Features/MenuImages/DeleteMenuImage.cs
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Menu.API.Services;
using Menu.API.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menu.API.Features.MenuImages
{
    // --- COMMAND ---
    public sealed record DeleteMenuImageCommand(Guid MenuId, string Url) : ICommand<Unit>;

    // --- VALIDATOR ---
    public class DeleteMenuImageCommandValidator : AbstractValidator<DeleteMenuImageCommand>
    {
        private readonly ApplicationDbContext _db;

        public DeleteMenuImageCommandValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.MenuId)
                .NotEmpty().WithMessage("Menu ID is required.");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Url is required.")
                .MustAsync(ImageBelongsToMenu).WithMessage("Image not found or does not belong to the specified menu.");
        }

        private async Task<bool> ImageBelongsToMenu(DeleteMenuImageCommand command, string url, CancellationToken cancellationToken)
        {
            return await _db.MenuImages.AnyAsync(mi => mi.Url == url && mi.MenuId == command.MenuId, cancellationToken);
        }
    }

    // --- HANDLER ---
    public class DeleteMenuImageHandler(ApplicationDbContext db, IFileStorageService fileStorageService) : ICommandHandler<DeleteMenuImageCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteMenuImageCommand request, CancellationToken cancellationToken)
        {
            var menuImage = await db.MenuImages
                .Include(mi => mi.Menu)
                .FirstOrDefaultAsync(mi => mi.Url == request.Url && mi.MenuId == request.MenuId, cancellationToken)
                ?? throw new NotFoundException($"Menu Image", $"URL {request.Url} for Menu ID {request.MenuId}");

            // Hapus file dari storage
            var fileName = Path.GetFileName(menuImage.Url);
            var folderName = $"menu-images/{menuImage.MenuId}"; // Gunakan folder yang sama dengan saat upload
            await fileStorageService.DeleteFileAsync(fileName, folderName);

            // Jika gambar yang dihapus adalah thumbnail utama, reset Menu.ImageUrl
            if (menuImage.IsThumbnail)
            {
                var menu = menuImage.Menu; // Menu sudah di-load via Include
                if (menu != null && menu.ImageUrl == menuImage.Url) // Pastikan URL sama
                {
                    menu.ImageUrl = null; // Reset thumbnail utama
                    db.Menus.Update(menu);
                }
            }

            // Hapus record dari database
            db.MenuImages.Remove(menuImage);
            await db.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}