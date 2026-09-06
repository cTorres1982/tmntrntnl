using JobTracker.SharedKernel.Domain;

namespace JobTracker.Modules.Billing.Domain;

/// <summary>
/// Deliberately minimal: AC.md's Billing scope is "another bounded context that
/// reacts to a completed job", not a pricing/line-items model — no such model is
/// described anywhere in the assessment, so none is invented here. What matters for
/// the assessment is the cross-module reaction and the idempotency guarantee.
/// </summary>
public sealed class Invoice : AggregateRoot
{
    private Invoice()
    {
        IdempotencyKey = string.Empty;
    }

    private Invoice(Guid id, Guid jobId, Guid organizationId, Guid customerId, DateTime jobCompletedAtUtc, DateTime issuedAtUtc)
        : base(id)
    {
        JobId = jobId;
        OrganizationId = organizationId;
        CustomerId = customerId;
        JobCompletedAtUtc = jobCompletedAtUtc;
        IssuedAtUtc = issuedAtUtc;
        IdempotencyKey = BuildIdempotencyKey(jobId, jobCompletedAtUtc);
    }

    public Guid JobId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid CustomerId { get; private set; }

    public DateTime JobCompletedAtUtc { get; private set; }

    public DateTime IssuedAtUtc { get; private set; }

    /// <summary>
    /// JobId + CompletedAt, per AC.md 3.4.2 — the same (JobId, CompletedAtUtc) pair
    /// always produces the same key, so re-processing the same
    /// JobCompletedIntegrationEvent (at-least-once delivery from the outbox can
    /// redeliver) can never create a second Invoice for the same completion.
    /// </summary>
    public string IdempotencyKey { get; private set; }

    public static Invoice Create(Guid jobId, Guid organizationId, Guid customerId, DateTime jobCompletedAtUtc, DateTime issuedAtUtc) =>
        new(Guid.NewGuid(), jobId, organizationId, customerId, jobCompletedAtUtc, issuedAtUtc);

    public static string BuildIdempotencyKey(Guid jobId, DateTime jobCompletedAtUtc) =>
        $"{jobId:N}-{jobCompletedAtUtc:O}";
}
