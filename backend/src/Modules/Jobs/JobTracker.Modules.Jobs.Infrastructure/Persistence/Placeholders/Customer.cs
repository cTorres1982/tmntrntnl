namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Placeholders;

/// <summary>
/// Minimal stand-in for the out-of-scope Contacts module (see AC.md Part 4.3, which
/// discusses denormalizing from/joining a hypothetical Contacts module). Exists only
/// so Job.CustomerId has a real FK target and seed/demo data. No behavior.
/// </summary>
public sealed class Customer
{
    public required Guid Id { get; init; }

    public required Guid OrganizationId { get; init; }

    public required string Name { get; init; }
}
