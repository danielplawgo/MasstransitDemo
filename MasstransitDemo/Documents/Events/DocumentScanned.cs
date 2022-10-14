namespace MasstransitDemo.Documents.Events;

public record DocumentScanned(Guid Id) : IEvent;