using System.Net.Mail;
using ay.Aspire.For.MailKit.Client;
using MailKit.Net.Smtp;
using MimeKit;

namespace ay.PoC.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();

        //Configure Swagger
        builder.Services.AddSwaggerGen();

        // Add services to the container.
        builder.AddMailKitClient("maildev");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapPost("/subscribe",
            async (MailKitClientFactory factory, string email) =>
            {
                ISmtpClient client = await factory.GetSmtpClientAsync();

                using var message = new MailMessage("newsletter@yourcompany.com", email)
                {
                    Subject = "Welcome to our newsletter!",
                    Body = "Thank you for subscribing to our newsletter!"
                };

                await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
            });

        app.MapPost("/unsubscribe",
            async (MailKitClientFactory factory, string email) =>
            {
                ISmtpClient client = await factory.GetSmtpClientAsync();

                using var message = new MailMessage("newsletter@yourcompany.com", email)
                {
                    Subject = "You are unsubscribed from our newsletter!",
                    Body = "Sorry to see you go. We hope you will come back soon!"
                };

                await client.SendAsync(MimeMessage.CreateFromMailMessage(message));
            });

        app.Run();
    }
}
