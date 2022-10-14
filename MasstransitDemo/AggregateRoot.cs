using MassTransit;

namespace MasstransitDemo
{
    public abstract class AggregateRoot
    {
        private readonly List<IEvent> _events = new List<IEvent>();
        public IEnumerable<IEvent> Events => _events;

        protected void AddEvent(IEvent @event)
        {
            _events.Add(@event);
        }
    }

    [ExcludeFromTopology]
    public interface IEvent
    {
    }
}
