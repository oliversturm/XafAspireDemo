using XafAspireDemo.DemoService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add service defaults (telemetry, health checks, etc.)
builder.Services.AddAspireServiceDefaults();
builder.Services.ConfigureOpenTelemetry(builder.Configuration, builder.Environment);

var telemetry = new Telemetry();
builder.Services.AddSingleton(telemetry);

builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("XafAspireDemo.DemoService"))
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(telemetry.MeterName);
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet(
        "/important-business-value",
        (Telemetry telemetry) =>
        {
            // Start the activity for distributed tracing
            using var activity = telemetry.ActivitySource.StartActivity(
                "ImportantBusinessValueRetrieval"
            );

            // Simulate some work by generating a random number
            var importantBusinessValue = Random.Shared.Next(1, 10000);

            // Record the business value retrieval count in telemetry
            telemetry.ImportantBusinessValueRetrievalCounter.Add(1);

            // When the activity is disposed (at the end of this scope),
            // its duration will be automatically recorded in the tracing system
            // We can also record it explicitly in our metrics system
            if (activity != null)
            {
                // Record the duration in our histogram when the activity completes
                activity.Stop();
                var durationMs = activity.Duration.TotalMilliseconds;
                telemetry.ImportantBusinessValueRetrievalDuration.Record(durationMs);
            }

            return Results.Ok(new { ImportantBusinessValue = importantBusinessValue });
        }
    )
    .WithName("GetImportantBusinessValue")
    .WithOpenApi();

// Add Aspire default endpoints (health checks, etc.)
app.MapDefaultAspireDevEndpoints();

app.Run();
