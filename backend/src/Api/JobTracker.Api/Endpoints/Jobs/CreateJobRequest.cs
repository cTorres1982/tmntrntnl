namespace JobTracker.Api.Endpoints.Jobs;

/// <summary>
/// Deliberately has no OrganizationId field — the tenant is always derived
/// server-side from ICurrentOrganizationProvider (populated by
/// TenantContextMiddleware), never trusted from the request body.
/// </summary>
public sealed record CreateJobRequest(
    string Title,
    string Description,
    string Street,
    string City,
    string State,
    string ZipCode,
    double Latitude,
    double Longitude,
    Guid CustomerId,
    string? Notes,
    DateTime? ScheduledDate,
    Guid? AssigneeId);
