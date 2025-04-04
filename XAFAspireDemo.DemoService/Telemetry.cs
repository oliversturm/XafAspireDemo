using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace XafAspireDemo.DemoService
{
    public class Telemetry : IDisposable
    {
        public ActivitySource ActivitySource { get; }
        public Meter Meter { get; }
        public string MeterName => Meter.Name;
        public Counter<long> ImportantBusinessValueRetrievalCounter { get; }
        public Histogram<double> ImportantBusinessValueRetrievalDuration { get; }

        public Telemetry(string serviceName = "XafAspireDemo.DemoService", string version = "1.0.0")
        {
            ActivitySource = new ActivitySource(serviceName, version);
            Meter = new Meter(serviceName, version);

            ImportantBusinessValueRetrievalCounter = Meter.CreateCounter<long>(
                "important_business_value.retrieval_count"
            );
            ImportantBusinessValueRetrievalDuration = Meter.CreateHistogram<double>(
                "important_business_value.retrieval_duration"
            );
        }

        public void Dispose()
        {
            ActivitySource.Dispose();
            Meter.Dispose();
        }
    }
}
