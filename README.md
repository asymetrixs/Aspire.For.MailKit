# ay.Aspire.For.MailKit

This project implements [Create custom .NET Aspire hosting integrations](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-hosting-integration)
and hooks up Aspire with MailKit as described.

## Integration

To integrate MailKit with Aspire use the following two examples.
The integration will spin up a MailDev container where you will receive your emails.

### Hosting

In your Aspire project integrate MailKit like so:

```csharp

var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("maildev");

var webApi = builder.AddProject<[REPLACE WITH YOUR PROJECT]>("webApi")
    .WithReference(maildev);

builder.Build().Run();

```


### Client

In your client that is hosted by Aspire, integrate like so:

```csharp
builder.AddMailKitClient("maildev");
```
