using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace XafAspireDemo.Blazor.Server
{
    public class Telemetry : IDisposable
    {
        public ActivitySource ActivitySource { get; }
        public Meter Meter { get; }
        public string MeterName => Meter.Name;
        public Counter<long> ImportantBusinessOperationCounter { get; }
        public Histogram<double> ImportantBusinessOperationDuration { get; }

        public Telemetry(
            string serviceName = "XafAspireDemo.Blazor.Server",
            string version = "1.0.0"
        )
        {
            ActivitySource = new ActivitySource(serviceName, version);
            Meter = new Meter(serviceName, version);

            ImportantBusinessOperationCounter = Meter.CreateCounter<long>(
                "important_business_operation.execution_count"
            );
            ImportantBusinessOperationDuration = Meter.CreateHistogram<double>(
                "important_business_operation.execution_duration"
            );
        }

        public void Dispose()
        {
            ActivitySource.Dispose();
            Meter.Dispose();
        }
    }
}
