using MassTransit;

namespace MasstransitDemo.Infrastructure
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IBus _publishEndpoint;

        public EventPublisher(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
        }

        public async Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken) where T : IEvent
        {
            foreach (var @event in events)
            {
                await PublishAsync(@event, cancellationToken);
            }
        }
    }

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent;

        Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken) where T : IEvent;
    }
}
