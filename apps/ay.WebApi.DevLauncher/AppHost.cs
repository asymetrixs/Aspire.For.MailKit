using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("maildev");

var webApi = builder.AddProject<ay_PoC_WebApi>("webApi")
    .WithReference(maildev);

builder.Build().Run();
