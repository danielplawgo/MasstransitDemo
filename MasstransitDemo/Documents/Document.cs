using MassTransit;
using MasstransitDemo.Documents.Events;

namespace MasstransitDemo.Documents
{
    public class Document : AggregateRoot
    {
        public NewId Id { get; }

        public string Name { get; private set; }

        public DocumentStatus Status { get; private set; }

        public Document(NewId id, string name, DocumentStatus status)
        {
            Id = id;
            Name = name;
            Status = status;
        }

        public static Document Create(string name)
        {
            var document = new Document(NewId.Next(), name, DocumentStatus.Created);
            document.AddEvent(new DocumentCreated(document.Id.ToGuid()));
            return document;
        }

        public void MarkAsUploaded()
        {
            Status = DocumentStatus.Uploaded;

            AddEvent(new DocumentUploaded(Id.ToGuid()));    
        }

        public void MarkAsScanned()
        {
            Status = DocumentStatus.Scanned;

            AddEvent(new DocumentScanned(Id.ToGuid()));    
        }
    }

    public enum DocumentStatus
    {
        Created,
        Uploaded,
        Scanned
    }
}
