var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("XafAspireDemoDb");

var demoService = builder
    .AddProject<Projects.XAFAspireDemo_DemoService>("demoservice")
    .WithHttpsEndpoint(name: "demoservice-https");

builder
    .AddProject<Projects.XafAspireDemo_Blazor_Server>("xafaspiredemo-blazor-server")
    .WithEnvironment("ASPIRE_DEBUG", "true")
    .WithHttpsEndpoint(name: "xafaspiredemo-blazor-server-https")
    .WithReference(db)
    .WithReference(demoService)
    .WaitFor(db);

builder.Build().Run();
