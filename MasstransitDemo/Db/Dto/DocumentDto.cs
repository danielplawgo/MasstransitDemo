using MassTransit;
using MasstransitDemo.Documents;

namespace MasstransitDemo.Db.Dto
{
    public class DocumentDto : IDtoModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DocumentStatus Status { get; set; }
    }

    public static class DocumentDtoMapper
    {
        public static DocumentDto AsDto(this Document document, DocumentDto? dto = null)
        {
            dto ??= new DocumentDto();

            dto.Id = document.Id.ToGuid();
            dto.Name = document.Name;
            dto.Status = document.Status;

            return dto;
        }

        public static Document AsEntity(this DocumentDto dto)
        {
            return new Document(dto.Id.ToNewId(),
                dto.Name,
                dto.Status);
        }
    }
}
