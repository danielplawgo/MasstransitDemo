using System.Collections.Concurrent;
using System.Diagnostics;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SPO.Tcp.Server.Telemetries
{
    public class TelemetryReceiveObserver : IReceiveObserver
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly ConcurrentDictionary<string, IOperationHolder<RequestTelemetry>> _operations = new ConcurrentDictionary<string, IOperationHolder<RequestTelemetry>>();

        public TelemetryReceiveObserver()
        {
            _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
        }

        public Task PreReceive(ReceiveContext context)
        {
            var serviceBusContext = (ServiceBusReceiveContext)context;

            var diagnosticId = serviceBusContext.Properties["Diagnostic-Id"];

            if (diagnosticId != null)
            {
                var requestActivity = new Activity("Masstransit Process Message");
                requestActivity.SetParentId(diagnosticId.ToString());
                var operation = _telemetryClient.StartOperation<RequestTelemetry>(requestActivity);

                _operations.TryAdd(serviceBusContext.MessageId, operation);
            }

            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            var serviceBusContext = (ServiceBusReceiveContext)context;

            if (_operations.TryGetValue(serviceBusContext.MessageId, out var operation))
            {
                operation.Telemetry.Success = true;
                operation.Dispose();
                _operations.TryRemove(serviceBusContext.MessageId, out operation);
            }

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            _telemetryClient.TrackException(exception);
            var serviceBusContext = (ServiceBusReceiveContext)context;

            if (_operations.TryGetValue(serviceBusContext.MessageId, out var operation))
            {
                operation.Telemetry.ResponseCode = "Fail";
                operation.Telemetry.Success = false;
                operation.Dispose();
                _operations.TryRemove(serviceBusContext.MessageId, out operation);
            }

            return Task.CompletedTask;
        }
    }
}
