using Menu.API.Data;

namespace Menu.API.Handler
{
    public sealed record DeleteMenuItemCommand(Guid Id) : ICommand<bool>;

    public sealed class DeleteMenuItemHandler(IMenuItemRepository repo)
        : ICommandHandler<DeleteMenuItemCommand, bool>
    {
        public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
        {
            var result = await repo.DeleteAsync(request.Id, cancellationToken);

            return result;
        }
    }
}