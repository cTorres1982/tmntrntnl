namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Placeholders;

/// <summary>
/// Minimal stand-in for the out-of-scope crew/staff module — AC.md marks
/// Job.AssigneeId as "(FK)" but never asks for that module to be built. Exists only
/// so the FK constraint and seed/demo data are real. No behavior, not part of the
/// Jobs bounded context's domain model — hence it lives in Infrastructure, not Domain.
/// </summary>
public sealed class Assignee
{
    public required Guid Id { get; init; }

    public required Guid OrganizationId { get; init; }

    public required string Name { get; init; }
}
