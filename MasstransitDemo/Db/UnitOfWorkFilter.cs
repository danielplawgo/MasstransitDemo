using MassTransit;

namespace MasstransitDemo.Db
{
    public class UnitOfWorkFilter<T> : IFilter<ConsumeContext<T>>
        where T : class
    {
        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            await next.Send(context);

            context.TryGetPayload(out IServiceProvider? serviceProvider);
            if (serviceProvider == null)
            {
                return;
            }

            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            //throw new Exception("test");
            //await dataContext.SaveChangesAsync();
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
