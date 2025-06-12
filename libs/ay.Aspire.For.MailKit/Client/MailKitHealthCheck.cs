using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ay.Aspire.For.MailKit.Client;

internal sealed class MailKitHealthCheck(
    MailKitClientFactory factory,
    ILogger<MailKitHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // The factory connects (and authenticates).
            _ = await factory.GetSmtpClientAsync(cancellationToken);

            logger.LogDebug("Connection to mail server is healthy.");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Connection to mail server is unhealthy.");

            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
