using BuildingBlocks.Exceptions;
using FluentValidation;
using Menu.API.Data;

namespace Menu.API.Handler;

public sealed record CreateMenuItemCommand(CreateMenuItemDto Dto) : ICommand<MenuItemDto>;

public class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator(AppDbContext db)
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MustAsync(async (name, ct) =>
                !await db.MenuItems.AnyAsync(m => m.Name == name, ct))
            .WithMessage("Menu name must be unique.");

        RuleFor(x => x.Dto.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.Dto.Carbo)
            .GreaterThanOrEqualTo(0).WithMessage("Carbo must be >= 0.");

        RuleFor(x => x.Dto.Protein)
            .GreaterThanOrEqualTo(0).WithMessage("Protein must be >= 0.");

        RuleFor(x => x.Dto.Categories)
            .NotEmpty().WithMessage("At least one category is required.");
    }
}

public sealed class CreateMenuItemHandler(IMenuItemRepository repo)
    : ICommandHandler<CreateMenuItemCommand, MenuItemDto>
{
    public async Task<MenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menu = request.Dto.Adapt<MenuItem>();
        await repo.AddAsync(menu, cancellationToken);
        return menu.Adapt<MenuItemDto>();
    }
}