using BuildingBlocks.Exceptions;
using FluentValidation;
using Menu.API.Data;

namespace Menu.API.Handler
{
    public sealed record UpdateMenuItemCommand(UpdateMenuItemDto Dto) : ICommand<MenuItemDto?>;

    public class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
    {
        public UpdateMenuItemCommandValidator(AppDbContext db)
        {
            RuleFor(x => x.Dto.Id)
                .NotEmpty().WithMessage("Id is required.")
                .MustAsync(async (id, ct) =>
                    await db.MenuItems.AnyAsync(m => m.Id == id, ct))
                .WithMessage("Menu item not found.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MustAsync(async (command, name, ct) =>
                    !await db.MenuItems
                        .AnyAsync(m => m.Name == name && m.Id != command.Dto.Id, ct))
                .WithMessage("Menu name must be unique.");

            RuleFor(x => x.Dto.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0.");

            RuleFor(x => x.Dto.Carbo)
                .GreaterThanOrEqualTo(0).WithMessage("Carbo must be >= 0.");

            RuleFor(x => x.Dto.Protein)
                .GreaterThanOrEqualTo(0).WithMessage("Protein must be >= 0.");

            RuleFor(x => x.Dto.Categories)
                .NotEmpty().WithMessage("At least one category is required.");
        }
    }

    public sealed class UpdateMenuItemHandler(IMenuItemRepository repo)
     : ICommandHandler<UpdateMenuItemCommand, MenuItemDto?>
    {
        public async Task<MenuItemDto?> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            var updated = await repo.UpdateAsync(request.Dto, cancellationToken);
            return updated?.Adapt<MenuItemDto>();
        }
    }
}