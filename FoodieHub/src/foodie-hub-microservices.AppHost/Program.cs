var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Auth_API>("auth-api");

builder.AddProject<Projects.Menu_API>("menu-api");

builder.Build().Run();