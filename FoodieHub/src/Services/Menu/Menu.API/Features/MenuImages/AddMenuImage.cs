// Menu.API/Features/MenuImages/AddMenuImage.cs
using BuildingBlocks.CQRS;
using FluentValidation;
using Mapster;
using Menu.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menu.API.Features.MenuImages
{
    // --- COMMAND REQUEST ---
    public record AddMenuImageRequest(

        Guid MenuId,
        IFormFile ImageFile
    );

    // --- COMMAND ---
    public sealed record AddMenuImageCommand(AddMenuImageRequest Request) : ICommand<MenuImageDto>;

    // --- VALIDATOR ---
    public class AddMenuImageCommandValidator : AbstractValidator<AddMenuImageCommand>
    {
        private readonly ApplicationDbContext _db;

        public AddMenuImageCommandValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.Request).NotNull().WithMessage("Request data must not be null.");

            RuleFor(x => x.Request.MenuId)
                .NotEmpty().WithMessage("Menu ID is required.")
                .MustAsync(MenuMustExist).WithMessage("Menu with this ID does not exist.");

            RuleFor(x => x.Request.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => file.Length > 0).WithMessage("Image file cannot be empty.")
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("Image file size cannot exceed 5MB.") // Contoh batasan 5MB
                .Must(file => IsImage(file)).WithMessage("Only image files (JPG, PNG, GIF) are allowed.");
        }

        private async Task<bool> MenuMustExist(Guid menuId, CancellationToken cancellationToken)
        {
            return await _db.Menus.AnyAsync(m => m.Id == menuId, cancellationToken);
        }

        private bool IsImage(IFormFile file)
        {
            if (file == null) return false;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }

    // --- HANDLER ---
    public class AddMenuImageHandler(ApplicationDbContext db, IFileStorageService fileStorageService) : ICommandHandler<AddMenuImageCommand, MenuImageDto>
    {
        public async Task<MenuImageDto> Handle(AddMenuImageCommand request, CancellationToken cancellationToken)
        {
            var menu = await db.Menus
                .Include(m => m.Images) // Load existing images
                .FirstOrDefaultAsync(m => m.Id == request.Request.MenuId, cancellationToken)
                ?? throw new MenuNotFoundException(request.Request.MenuId);

            // Upload the new image
            var imageUrl = await fileStorageService.UploadFileAsync(request.Request.ImageFile, $"menu-images/{menu.Id}"); // Folder per menu ID
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new InvalidOperationException("Failed to upload image file.");
            }

            // Create new MenuImage entity
            var newMenuImage = new MenuImage
            {
                MenuId = menu.Id,
                Url = imageUrl,
            };

            //// Handle thumbnail logic
            //if (request.Request.IsThumbnail)
            //{
            //    // Hapus IsThumbnail dari gambar lama yang merupakan thumbnail untuk menu ini
            //    var oldThumbnail = menu.Images.FirstOrDefault(i => i.IsThumbnail);
            //    if (oldThumbnail != null)
            //    {
            //        oldThumbnail.IsThumbnail = false;
            //        db.MenuImages.Update(oldThumbnail);
            //    }
            //    // Update Menu.ImageUrl utama (thumbnail)
            //    menu.ImageUrl = imageUrl;
            //    db.Menus.Update(menu);
            //}

            db.MenuImages.Add(newMenuImage);
            await db.SaveChangesAsync(cancellationToken);

            return newMenuImage.Adapt<MenuImageDto>();
        }
    }
}