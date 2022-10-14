using MassTransit;
using MasstransitDemo.Db.Repositories;
using MasstransitDemo.Infrastructure;

namespace MasstransitDemo.Documents.Jobs
{
    public static class UploadDocumentFile
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
                var transactionContext = context.GetPayload<TransactionContext>();

                var document = await _repository.Get(context.Message.Id, context.CancellationToken);

                document.MarkAsUploaded();

                //throw new Exception("test");

                await _repository.Update(document, context.CancellationToken);

                await _eventPublisher.PublishAsync(document.Events, context.CancellationToken);
            }
        }

        public class Fault : IConsumer<Fault<Command>>
        {
            public Task Consume(ConsumeContext<Fault<Command>> context)
            {
                var contextMessage = context.Message;

                return Task.CompletedTask;
            }
        }
    }
}
