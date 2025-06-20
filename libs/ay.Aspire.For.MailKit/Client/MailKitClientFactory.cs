using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace ay.Aspire.For.MailKit.Client;

/// <summary>
/// A factory for creating <see cref="ISmtpClient"/> instances
/// given a <paramref name="smtpUri"/> (and optional <paramref name="credentials"/>).
/// </summary>
/// <param name="settings">
/// The <see cref="MailKitClientSettings"/> settings for the SMTP server
/// </param>
public sealed class MailKitClientFactory(
    MailKitClientSettings settings,
    ILogger<MailKitClientFactory> logger)
{
    /// <summary>
    /// Gets an <see cref="ISmtpClient"/> instance in the connected state
    /// (and that's been authenticated if configured).
    /// </summary>
    /// <param name="cancellationToken">Used to abort client creation and connection.</param>
    /// <returns>A connected (and authenticated) <see cref="ISmtpClient"/> instance.</returns>
    /// <remarks>
    /// Since both the connection and authentication are considered expensive operations,
    /// the <see cref="ISmtpClient"/> returned is intended to be used for the duration of a request
    /// (registered as 'Scoped') and is automatically disposed of.
    /// </remarks>
    public async Task<ISmtpClient> GetSmtpClientAsync(CancellationToken cancellationToken = default)
    {
        var client = new SmtpClient();
        try
        {
            logger.LogDebug("Connecting to SMTP server: {Endpoint}", settings.Endpoint);

            if (settings.Endpoint is not null)
            {
                await client
                    .ConnectAsync(settings.Endpoint, cancellationToken)
                    .ConfigureAwait(false);

                logger.LogDebug("Connected to SMTP server: {Endpoint}", settings.Endpoint);

                if ((client.Capabilities & SmtpCapabilities.Authentication) != 0
                    && settings.Username is not null)
                {
                    await client
                        .AuthenticateAsync(settings.Username, settings.Password, cancellationToken)
                        .ConfigureAwait(false);

                    logger.LogDebug("Authenticated to SMTP server: {Endpoint}", settings.Endpoint);
                }
            }

            return client;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to SMTP server: {Endpoint}", settings.Endpoint);

            await client.DisconnectAsync(true, cancellationToken);
            client.Dispose();
            throw;
        }
    }
}
