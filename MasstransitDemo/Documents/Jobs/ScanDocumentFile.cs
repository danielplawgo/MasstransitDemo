using MassTransit;
using MasstransitDemo.Db.Repositories;
using MasstransitDemo.Documents.Events;
using MasstransitDemo.Infrastructure;

namespace MasstransitDemo.Documents.Jobs
{
    public static class ScanDocumentFile
    {
        public record Command(Guid Id);

        public class Handler : IConsumer<Command>
        {
            private readonly IDocumentRepository _repository;
            private readonly IEventPublisher _eventPublisher;

            public Handler(IDocumentRepository repository, IEventPublisher eventPublisher)
            {
                _repository = repository;
                _eventPublisher = eventPublisher;
            }

            public async Task Consume(ConsumeContext<Command> context)
            {
                var document = await _repository.Get(context.Message.Id, context.CancellationToken);

                document.MarkAsScanned();

                await _repository.Update(document, context.CancellationToken);

                await _eventPublisher.PublishAsync(document.Events, context.CancellationToken);
            }
        }
    }
}
