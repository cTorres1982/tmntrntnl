namespace JobTracker.SharedKernel.Multitenancy;

/// <summary>
/// Read-only view of "which tenant is this request for" — implemented by the Api's
/// TenantContextMiddleware-populated provider in production, and consumed by every
/// module's DbContext to apply a global query filter on OrganizationId. This is
/// defense-in-depth on top of the OrganizationId every command/query already takes
/// explicitly — a handler that forgets to filter still can't leak cross-tenant rows.
/// </summary>
public interface ICurrentOrganizationProvider
{
    Guid OrganizationId { get; }
}
