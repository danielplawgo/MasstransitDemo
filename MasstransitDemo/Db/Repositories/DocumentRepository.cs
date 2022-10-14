using MasstransitDemo.Db.Dto;
using MasstransitDemo.Documents;

namespace MasstransitDemo.Db.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDtoRepository<DocumentDto> _repository;

        public DocumentRepository(IDtoRepository<DocumentDto> repository)
        {
            _repository = repository;
        }

        public async Task Add(Document document, CancellationToken cancellationToken)
        {
            var dto = document.AsDto();
            await _repository.Add(dto, cancellationToken);
        }

        public async Task Update(Document document, CancellationToken cancellationToken)
        {
            var dto = await _repository.Get(document.Id.ToGuid(), cancellationToken);

            dto = document.AsDto(dto);

            await _repository.Update(dto, cancellationToken);
        }

        public async Task<Document> Get(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _repository.Get(id, cancellationToken);

            return dto.AsEntity();
        }
    }

    public interface IDocumentRepository
    {
        Task Add(Document document, CancellationToken cancellationToken);
        Task Update(Document document, CancellationToken cancellationToken);
        Task<Document> Get(Guid id, CancellationToken cancellationToken);
    }
}
