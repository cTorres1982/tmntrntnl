namespace JobTracker.Modules.Jobs.Infrastructure.Notifications;

public sealed class SendGridOptions
{
    public const string SectionName = "SendGrid";

    public string? ApiKey { get; set; }

    public string FromEmail { get; set; } = "no-reply@jobtracker.example.com";

    public string FromName { get; set; } = "JobTracker";
}
