using System.Transactions;
using MassTransit;
using MasstransitDemo.Db;
using MasstransitDemo.Db.Dto;
using MasstransitDemo.Db.Repositories;
using MasstransitDemo.Documents;
using MasstransitDemo.Documents.Events;
using MasstransitDemo.Infrastructure;
using MasstransitDemo.Telemetries;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using SPO.Tcp.Server.Telemetries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<DataContext>(o =>
    {
        o.UseSqlServer();

        o.UseBusOutbox();
    });

    x.AddServiceBusMessageScheduler();

    x.AddSagaStateMachine<DocumentSaga, DocumentSagaState, DocumentSagaDefinition>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;

            r.ExistingDbContext<DataContext>();
        });

    x.UsingAzureServiceBus((context,
        cfg) =>
    {
        cfg.UseTransaction(c =>
        {
            c.IsolationLevel = IsolationLevel.ReadCommitted;
        });

        cfg.UseServiceBusMessageScheduler();

        cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));

        cfg.ConfigureEndpoints(context);

        cfg.ConnectReceiveObserver(new TelemetryReceiveObserver());

        //cfg.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));
        //cfg.UseScheduledRedelivery(r => r.Interval(10, TimeSpan.FromSeconds(30)));

        //cfg.UseConsumeFilter(typeof(UnitOfWorkFilter<>), context);
    });

    x.AddConsumers(typeof(Program).Assembly);

    MessageCorrelation.UseCorrelationId<DocumentCreated>(x => x.Id);
    MessageCorrelation.UseCorrelationId<DocumentUploaded>(x => x.Id);
    MessageCorrelation.UseCorrelationId<DocumentScanned>(x => x.Id);
});

builder.Services.AddMediator(x =>
{
    x.AddConsumers(typeof(Program).Assembly);

    x.ConfigureMediator((context, cfg) =>
    {
        //cfg.UseConsumeFilter(typeof(UnitOfWorkFilter<>), context);

        cfg.UseTransaction(c =>
        {
            c.IsolationLevel = IsolationLevel.ReadCommitted;
        });

    });

});

builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((host,
    log) =>
{
    if (host.HostingEnvironment.IsProduction())
        log.MinimumLevel.Information();
    else
        log.MinimumLevel.Debug();

    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
    //log.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information);
    log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
    log.WriteTo.Console();
#if DEBUG
    log.WriteTo.Seq("http://localhost:5341");
#endif
});

builder.Services.AddTelemetry(builder.Configuration);

builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped(typeof(IDtoRepository<>), typeof(DtoRepository<>));
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger()
    .UseSwaggerUI();

app.Run();