var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("XafAspireDemoDb");

builder
    .AddProject<Projects.XafAspireDemo_Blazor_Server>("xafaspiredemo-blazor-server")
    .WithEnvironment("ASPIRE_DEBUG", "true")
    .WithHttpsEndpoint()
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
