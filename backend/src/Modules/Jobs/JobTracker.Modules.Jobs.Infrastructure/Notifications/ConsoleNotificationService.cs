using JobTracker.Modules.Jobs.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace JobTracker.Modules.Jobs.Infrastructure.Notifications;

/// <summary>
/// Fake used for local development/testing when no SendGrid API key is configured —
/// logs instead of sending, so the app builds and runs end-to-end without real
/// credentials.
/// </summary>
internal sealed class ConsoleNotificationService : INotificationService
{
    private readonly ILogger<ConsoleNotificationService> _logger;

    public ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[fake email] To: {To} | Subject: {Subject} | Body: {Body}",
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}
