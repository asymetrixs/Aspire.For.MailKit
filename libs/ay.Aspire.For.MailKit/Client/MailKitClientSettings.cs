using System.Data.Common;

namespace ay.Aspire.For.MailKit.Client;

/// <summary>
/// Provides the client configuration settings for connecting MailKit to an SMTP server.
/// </summary>
public sealed class MailKitClientSettings
{
    internal const string DefaultConfigSectionName = "MailKit:Client";

    /// <summary>
    /// Gets or sets the SMTP server <see cref="Uri"/>.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Username for authentication with the SMTP server.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for authentication with the SMTP server.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableTracing { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry metrics are disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableMetrics { get; set; }

    internal void ParseConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"""
                                                 ConnectionString is missing.
                                                 It should be provided in 'ConnectionStrings:<connectionName>'
                                                 or '{DefaultConfigSectionName}:Endpoint' key.'
                                                 configuration section.
                                                 """);
        }

        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
        {
            Endpoint = uri;
            return;
        }


        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue("Endpoint", out var endpoint))
        {
            throw new InvalidOperationException($"""
                                                 The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                                                 '{DefaultConfigSectionName}') is missing.
                                                 """);
        }

        if (!Uri.TryCreate(endpoint.ToString(), UriKind.Absolute, out uri))
        {
            throw new InvalidOperationException($"""
                                                 The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                                                 '{DefaultConfigSectionName}') isn't a valid URI.
                                                 """);
        }

        Endpoint = uri;

        if (builder.TryGetValue("Username", out var username))
        {
            Username = username.ToString();
        }

        if (builder.TryGetValue("Password", out var password))
        {
            Password = password.ToString();
        }

        if (builder.TryGetValue("DisableHealthChecks", out var confDisableHealthChecks)
            && bool.TryParse(confDisableHealthChecks.ToString()!, out var disableHealthChecks))
        {
            DisableHealthChecks = disableHealthChecks;
        }

        if (builder.TryGetValue("DisableTracing", out var confDisableTracing)
            && bool.TryParse(confDisableTracing.ToString()!, out var disableTracing))
        {
            DisableTracing = disableTracing;
        }

        if (builder.TryGetValue("DisableMetrics", out var confDisableMetrics)
            && bool.TryParse(confDisableMetrics.ToString()!, out var disableMetrics))
        {
            DisableMetrics = disableMetrics;
        }
    }
}
