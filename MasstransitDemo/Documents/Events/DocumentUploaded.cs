namespace MasstransitDemo.Documents.Events
{
    public record DocumentUploaded(Guid Id) : IEvent;
}
