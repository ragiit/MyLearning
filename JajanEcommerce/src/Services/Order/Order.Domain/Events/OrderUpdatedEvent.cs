using Order.Domain.Abstractions;
using Order.Domain.Models;

namespace Order.Domain.Events
{
    public record OrderUpdatedEvent(Models.Order Order) : IDomainEvent;
}