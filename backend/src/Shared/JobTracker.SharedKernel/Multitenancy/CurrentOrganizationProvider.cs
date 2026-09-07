namespace JobTracker.SharedKernel.Multitenancy;

/// <summary>
/// Mutable, scoped implementation of ICurrentOrganizationProvider — set once per
/// unit of work and read everywhere else through the read-only interface.
/// Two different callers set it, each appropriate to its own execution context:
/// the Api's TenantContextMiddleware (from the X-Organization-Id header, once per
/// HTTP request), and ProcessOutboxMessagesJob (from the domain event's own
/// OrganizationId, once per outbox message — a background job has no HTTP header
/// to read).
/// </summary>
public sealed class CurrentOrganizationProvider : ICurrentOrganizationProvider
{
    public Guid OrganizationId { get; set; }
}
