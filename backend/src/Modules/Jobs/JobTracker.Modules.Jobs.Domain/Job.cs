using JobTracker.Modules.Jobs.Domain.Events;
using JobTracker.SharedKernel.Domain;
using JobTracker.SharedKernel.Results;

namespace JobTracker.Modules.Jobs.Domain;

/// <summary>
/// Aggregate root. All state transitions go through domain methods that enforce
/// invariants and raise domain events — never through public setters.
/// </summary>
public sealed class Job : AggregateRoot
{
    private readonly List<JobPhoto> _photos = [];

    private Job()
    {
        Title = string.Empty;
        Description = string.Empty;
        Address = null!;
    }

    private Job(
        Guid id,
        string title,
        string description,
        Address address,
        Guid customerId,
        Guid organizationId,
        string? notes,
        DateTime utcNow)
        : base(id)
    {
        Title = title;
        Description = description;
        Address = address;
        CustomerId = customerId;
        OrganizationId = organizationId;
        Notes = notes;
        Status = JobStatus.Draft;
        CreatedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Address Address { get; private set; }

    public JobStatus Status { get; private set; }

    public DateTime? ScheduledDate { get; private set; }

    public Guid? AssigneeId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string? Notes { get; private set; }

    public DateTime? StartedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public string? CancellationReason { get; private set; }

    public string? SignatureUrl { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyList<JobPhoto> Photos => _photos.AsReadOnly();

    public static Result<Job> Create(
        string title,
        string description,
        Address address,
        Guid customerId,
        Guid organizationId,
        DateTime utcNow,
        string? notes = null,
        DateTime? scheduledDate = null,
        Guid? assigneeId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Job>(JobErrors.TitleRequired);
        }

        var job = new Job(Guid.NewGuid(), title, description, address, customerId, organizationId, notes, utcNow);

        if (scheduledDate is not null)
        {
            Result scheduleResult = job.Schedule(scheduledDate.Value, assigneeId, utcNow);
            if (scheduleResult.IsFailure)
            {
                return Result.Failure<Job>(scheduleResult.Error);
            }
        }

        job.RaiseDomainEvent(new JobCreatedDomainEvent(job.Id, job.OrganizationId, job.AssigneeId, utcNow));

        return job;
    }

    public Result Schedule(DateTime scheduledDate, Guid? assigneeId, DateTime utcNow)
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
        {
            return Result.Failure(JobErrors.TerminalState);
        }

        if (scheduledDate < utcNow)
        {
            return Result.Failure(JobErrors.ScheduledInPast);
        }

        if (assigneeId is null)
        {
            return Result.Failure(JobErrors.AssigneeRequired);
        }

        ScheduledDate = scheduledDate;
        AssigneeId = assigneeId;
        Status = JobStatus.Scheduled;
        UpdatedAtUtc = utcNow;

        return Result.Success();
    }

    public Result Start(DateTime utcNow)
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
        {
            return Result.Failure(JobErrors.TerminalState);
        }

        if (Status != JobStatus.Scheduled)
        {
            return Result.Failure(JobErrors.InvalidTransition);
        }

        Status = JobStatus.InProgress;
        StartedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;

        return Result.Success();
    }

    public Result Complete(DateTime utcNow, string signatureUrl)
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
        {
            return Result.Failure(JobErrors.TerminalState);
        }

        if (Status != JobStatus.InProgress)
        {
            return Result.Failure(JobErrors.InvalidTransition);
        }

        if (string.IsNullOrWhiteSpace(signatureUrl))
        {
            return Result.Failure(JobErrors.SignatureRequired);
        }

        Status = JobStatus.Completed;
        CompletedAtUtc = utcNow;
        SignatureUrl = signatureUrl;
        UpdatedAtUtc = utcNow;

        RaiseDomainEvent(new JobCompletedDomainEvent(Id, OrganizationId, CustomerId, AssigneeId, utcNow));

        return Result.Success();
    }

    public Result Cancel(string reason, DateTime utcNow)
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
        {
            return Result.Failure(JobErrors.TerminalState);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return Result.Failure(JobErrors.CancellationReasonRequired);
        }

        Status = JobStatus.Cancelled;
        CancelledAtUtc = utcNow;
        CancellationReason = reason;
        UpdatedAtUtc = utcNow;

        RaiseDomainEvent(new JobCancelledDomainEvent(Id, OrganizationId, reason, utcNow));

        return Result.Success();
    }

    public Result AddPhoto(string url, DateTime capturedAtUtc, string? caption)
    {
        if (Status is JobStatus.Completed or JobStatus.Cancelled)
        {
            return Result.Failure(JobErrors.TerminalState);
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure(JobErrors.PhotoUrlRequired);
        }

        _photos.Add(new JobPhoto(Guid.NewGuid(), url, capturedAtUtc, caption));

        return Result.Success();
    }
}
