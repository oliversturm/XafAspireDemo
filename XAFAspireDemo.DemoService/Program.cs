var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add service defaults (telemetry, health checks, etc.)
builder.Services.AddAspireServiceDefaults();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/important-business-value", () =>
{
    using var activity = telemetry.ActivitySource.StartActivity("ImportantBusinessAction");
    var importantBusinessValue = Random.Shared.Next(1, 10000);
    return Results.Ok(new { ImportantBusinessValue = importantBusinessValue });
})
.WithName("GetImportantBusinessValue")
.WithOpenApi();

// Add Aspire default endpoints (health checks, etc.)
app.MapDefaultAspireDevEndpoints();

app.Run();
