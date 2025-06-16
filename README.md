# ay.Aspire.For.MailKit

This project
implements [Create custom .NET Aspire hosting integrations](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-hosting-integration)
and hooks up Aspire with MailKit as described.

## Integration

To integrate MailKit with Aspire use the following example.
The integration will spin up a MailDev container where you will receive your emails.

The connection string is stored under 'ConnectionString:SmtpServer', either by using MailKit and environment variables
or by configuring it in the `appsettings.json`.
During the use of Aspire, the connection string will be provided in the environment variable
`ConnectionString__SmtpServer` by Aspire.

### Hosting

In your Aspire project integrate MailKit like so:

```csharp

var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("SmtpServer");

var webApi = builder.AddProject<[REPLACE WITH YOUR PROJECT]>("webApi")
    .WithReference(maildev);

builder.Build().Run();

```

### Client

In your client hosted by Aspire, integrate like so:

```csharp
builder.AddMailKitClient("SmtpServer");
```

Then inject the `MailKitClientFactory` like so into your controller or service. The factory will create an `SmtpClient`
instance that already authenticated to the server if the server supports authentication and if the username is not empty.
Make sure that you **dispose** the `SmtpClient` instance properly by using `using` or by calling `.Dispose()` on the instance.

```csharp
app.MapPost("/subscribe",
    async (MailKitClientFactory factory, string email) =>
    {
        using var message = new MailMessage("newsletter@yourcompany.com", email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };

        using var client = await factory.GetSmtpClientAsync();
        
        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
        await client.DisconnectAsync(true);
    });

```

And create your `ISmtpClient` to send an email.

To configure the connection to the SMTP server use

```json
{
  ...
  "ConnectionStrings": {
    "SmtpServer": "Endpoint=smtp://smtp.example.com:587;Username=foo;Password=bar;DisableHealthChecks=true;DisableTracing=true;DisableMetrics=true",
  },
  ...
}
```

In case you only have an endpoint without authentication, you can shorten it to

```json
{
  ...
  "ConnectionStrings": {
    "SmtpServer": "smtp://smtp.example.com:587",
  },
  ...
}
```

Alternatively use MailKit settings, but note that a ConnectionString will override the MailKit settings. 

```json
{
  ...
  "MailKit": {
    "Client": {
      "Endpoint": "smtp://smtp.example.com:587",
      "Username": "foo",
      "Password": "bar",
      "DisableHealthChecks": true,
      "DisableTracing": true,
      "DisableMetrics": true
    }
  }
  ...
}
```
