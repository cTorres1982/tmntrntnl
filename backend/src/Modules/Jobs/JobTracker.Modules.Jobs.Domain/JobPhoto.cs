using JobTracker.SharedKernel.Domain;

namespace JobTracker.Modules.Jobs.Domain;

/// <summary>
/// Belongs to a <see cref="Job"/> aggregate. The constructor is internal so it can only
/// be created from within this assembly — in practice, only via <see cref="Job.AddPhoto"/>.
/// </summary>
public sealed class JobPhoto : Entity
{
    internal JobPhoto(Guid id, string url, DateTime capturedAtUtc, string? caption)
        : base(id)
    {
        Url = url;
        CapturedAtUtc = capturedAtUtc;
        Caption = caption;
    }

    private JobPhoto()
    {
        Url = string.Empty;
    }

    public string Url { get; private set; }

    public DateTime CapturedAtUtc { get; private set; }

    public string? Caption { get; private set; }
}
