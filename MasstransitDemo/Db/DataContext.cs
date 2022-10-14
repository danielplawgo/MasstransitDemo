using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MasstransitDemo.Db.Dto;
using MasstransitDemo.Documents;
using Microsoft.EntityFrameworkCore;

namespace MasstransitDemo.Db;

public class DataContext : SagaDbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<DocumentDto> Documents { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new DocumentSagaStateMap(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}