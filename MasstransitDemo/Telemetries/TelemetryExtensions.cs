using Microsoft.ApplicationInsights.DependencyCollector;

namespace MasstransitDemo.Telemetries
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddTelemetry(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(o => configuration.GetSection("ApplicationInsights").Bind(o));

            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.EnableSqlCommandTextInstrumentation = true;
            });

            return services;
        }
    }
}
