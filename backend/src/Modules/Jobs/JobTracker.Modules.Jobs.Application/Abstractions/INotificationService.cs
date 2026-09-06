namespace JobTracker.Modules.Jobs.Application.Abstractions;

/// <summary>
/// Implemented by SendGridNotificationService (real) or ConsoleNotificationService
/// (fake, used when no SendGrid API key is configured) in the Infrastructure layer.
/// </summary>
public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
