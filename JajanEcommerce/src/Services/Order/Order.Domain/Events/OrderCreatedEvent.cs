using Order.Domain.Abstractions;
using Order.Domain.Models;

namespace Order.Domain.Events
{
    public record OrderCreatedEvent(Models.Order Order) : IDomainEvent;
}