using MassTransit;
using MassTransit.Mediator;
using MasstransitDemo.Db.Repositories;
using MasstransitDemo.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MasstransitDemo.Documents.Controllers
{
    [ApiController]
    [Route("documents")]
    public class CreateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CreateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Command command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return Accepted();
        }

        public record Command(string Name);

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
                var document = Document.Create(context.Message.Name);

                await _repository.Add(document, context.CancellationToken);

                await _eventPublisher.PublishAsync(document.Events, context.CancellationToken);

                //throw new Exception("test");
            }
        }
    }
}
