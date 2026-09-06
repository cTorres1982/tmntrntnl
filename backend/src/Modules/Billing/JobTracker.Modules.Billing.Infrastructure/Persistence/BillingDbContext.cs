using JobTracker.Modules.Billing.Domain;
using JobTracker.SharedKernel.Application;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence;

/// <summary>
/// SaveChangesAsync(CancellationToken) already matches IUnitOfWork's signature, so
/// DbContext satisfies that interface with no extra code — same as JobsDbContext.
/// </summary>
public sealed class BillingDbContext : DbContext, IUnitOfWork
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);
    }
}
