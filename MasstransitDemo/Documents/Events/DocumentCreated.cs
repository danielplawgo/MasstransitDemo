using MassTransit;

namespace MasstransitDemo.Documents.Events
{
    public record DocumentCreated(Guid Id) : IEvent;
}
