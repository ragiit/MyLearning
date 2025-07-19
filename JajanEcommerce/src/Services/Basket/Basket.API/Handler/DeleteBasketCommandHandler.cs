using Basket.API.Data;
using MediatR;

namespace Basket.API.Handler
{
    public record DeleteBasketCommand(string UserName) : IRequest<Unit>;

    public class DeleteBasketCommandHandler(IBasketRepository repository) : IRequestHandler<DeleteBasketCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
        {
            await repository.DeleteBasketAsync(request.UserName, cancellationToken);
            return Unit.Value;
        }
    }
}