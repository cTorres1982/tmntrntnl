using JobTracker.Modules.Jobs.Application.Abstractions;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace JobTracker.Modules.Jobs.Infrastructure.Notifications;

/// <summary>
/// Real implementation, used when a SendGrid API key is configured — see
/// DependencyInjection.AddJobsInfrastructure, which falls back to
/// ConsoleNotificationService otherwise so the app builds/runs without real
/// credentials.
/// </summary>
internal sealed class SendGridNotificationService : INotificationService
{
    private readonly ISendGridClient _client;
    private readonly SendGridOptions _options;

    public SendGridNotificationService(ISendGridClient client, IOptions<SendGridOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        SendGridMessage message = MailHelper.CreateSingleEmail(
            new EmailAddress(_options.FromEmail, _options.FromName),
            new EmailAddress(to),
            subject,
            plainTextContent: body,
            htmlContent: null);

        await _client.SendEmailAsync(message, cancellationToken);
    }
}
