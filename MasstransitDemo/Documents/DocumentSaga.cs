using MassTransit;
using MasstransitDemo.Db;
using MasstransitDemo.Documents.Events;
using MasstransitDemo.Documents.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasstransitDemo.Documents;

public class DocumentSagaState : SagaStateMachineInstance
{
    public int CurrentState { get; set; }

    public byte[] RowVersion { get; set; }

    public Guid CorrelationId { get; set; }
}

public class DocumentSaga : MassTransitStateMachine<DocumentSagaState>
{
    public DocumentSaga()
    {
        InstanceState(x => x.CurrentState, DocumentUploaded);

        Initially(
            When(DocumentCreatedEvent)
                .Then(c => c.Saga.CorrelationId = c.Message.Id)
                .TransitionTo(Initial)
                .Publish(c => new UploadDocumentFile.Command(c.Saga.CorrelationId))
        );

        During(Initial,
            When(DocumentUploadedEvent)
                .TransitionTo(DocumentUploaded)
                .Publish(c => new ScanDocumentFile.Command(c.Saga.CorrelationId))
        );

        During(DocumentUploaded,
            When(DocumentScannedEvent)
                .TransitionTo(Final));
    }

    public Event<DocumentCreated> DocumentCreatedEvent { get; }
    public Event<DocumentUploaded> DocumentUploadedEvent { get; }
    public State DocumentUploaded { get; set; }
    public Event<DocumentScanned> DocumentScannedEvent { get; }
}

public class DocumentSagaStateMap : SagaClassMap<DocumentSagaState>
{
    protected override void Configure(EntityTypeBuilder<DocumentSagaState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);

        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class DocumentSagaDefinition : SagaDefinition<DocumentSagaState>
{
    readonly IServiceProvider _provider;

    public DocumentSagaDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<DocumentSagaState> sagaConfigurator)
    {
        //endpointConfigurator.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
        //endpointConfigurator.UseScheduledRedelivery(r => r.Interval(5, TimeSpan.FromSeconds(30)));

        endpointConfigurator.UseEntityFrameworkOutbox<DataContext>(_provider);
    }
}