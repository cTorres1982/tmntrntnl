namespace JobTracker.SharedKernel.Multitenancy;

/// <summary>
/// Stand-in for `dotnet ef` design-time tooling, which builds the model outside of
/// any HTTP request — there is no real tenant to resolve, and none is needed just to
/// walk OnModelCreating and diff the migration snapshot.
/// </summary>
public sealed class NullCurrentOrganizationProvider : ICurrentOrganizationProvider
{
    public Guid OrganizationId => Guid.Empty;
}
