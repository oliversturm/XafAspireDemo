var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.XafAspireDemo_Blazor_Server>("xafaspiredemo-blazor-server");

builder.Build().Run();
