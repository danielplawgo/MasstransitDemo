using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MasstransitDemo.Db.Dto
{
    public class DtoRepository<T> : IDtoRepository<T> where T : class, IDtoModel, new()
    {
        private readonly DataContext _db;
        private readonly DbSet<T> _dbSet;
        private readonly ConsumeContext _consumeContext;

        public DtoRepository(DataContext db, ConsumeContext consumeContext)
        {
            _db = db;
            _consumeContext = consumeContext;
            _dbSet = _db.Set<T>();

            if (_db.Database.CurrentTransaction == null)
            {
                var transactionContext = consumeContext.GetPayload<TransactionContext>();

                //_db.Database.UseTransaction(transactionContext.Transaction);


            }
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken)
        {
            var item = await _dbSet.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (item == null)
            {
                throw new Exception("Item Not Found");
            }

            return item;
        }

        public async Task Add(T model, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(model, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(T model, CancellationToken cancellationToken)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            _db.Entry(new T() {Id = id}).State = EntityState.Deleted;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(m => m.Id == id, cancellationToken);
        }
    }

    public interface IDtoRepository<TModel> where TModel : class, IDtoModel, new()
    {
        Task<TModel> Get(Guid id, CancellationToken cancellationToken);

        Task Add(TModel model, CancellationToken cancellationToken);

        Task Update(TModel model, CancellationToken cancellationToken);

        Task Delete(Guid id, CancellationToken cancellationToken);

        Task<bool> Exists(Guid id, CancellationToken cancellationToken);
    }

    public interface IDtoModel
    {
        Guid Id { get; set; }
    }
}
