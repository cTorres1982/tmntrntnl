using JobTracker.Modules.Billing.Application.Abstractions;
using JobTracker.Modules.Billing.Domain;
using JobTracker.SharedKernel.Multitenancy;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence;

/// <summary>
/// SaveChangesAsync(CancellationToken) already matches IUnitOfWork's signature, so
/// DbContext satisfies that interface with no extra code — same as JobsDbContext.
/// </summary>
public sealed class BillingDbContext : DbContext, IUnitOfWork
{
    private readonly ICurrentOrganizationProvider _currentOrganizationProvider;

    public BillingDbContext(DbContextOptions<BillingDbContext> options, ICurrentOrganizationProvider currentOrganizationProvider)
        : base(options)
    {
        _currentOrganizationProvider = currentOrganizationProvider;
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);

        modelBuilder.Entity<Invoice>().HasQueryFilter(invoice => invoice.OrganizationId == _currentOrganizationProvider.OrganizationId);
    }
}
