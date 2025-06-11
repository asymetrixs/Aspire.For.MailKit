# ay.Aspire.For.MailKit

This project implements [Create custom .NET Aspire hosting integrations](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-hosting-integration)
and hooks up Aspire with MailKit as described.

## Integration

To integrate MailKit with Aspire use the following example.
The integration will spin up a MailDev container where you will receive your emails.

The connection string is stored under 'ConnectionString:SmtpServer', either by using MailKit and environment variables or by configuring it in the `appsettings.json`.
During the use of Aspire, the connection string will be provided in the environment variable `ConnectionString__SmtpServer` by Aspire.

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

In your client that is hosted by Aspire, integrate like so:

```csharp
builder.AddMailKitClient("SmtpServer");
```


Then inject the `MailKitClientFactory` like so into your controller or service:

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
        
        // Optionally authenticate but store these values somewhere safe.
        await client.AuthenticateAsync("someusername", "somepassword");
        
        await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
        await client.DisconnectAsync(true);
    });

```

And create your `ISmtpClient` to send an email.


To configure a real SMTP server use

```json
{
  ...
  "ConnectionStrings": {
    "SmtpServer": "smtp.example.com:587",
    "...": "..."
  },
  ...
}
```
