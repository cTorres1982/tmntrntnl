using EFCore.NamingConventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` construct BillingDbContext at design time without
/// needing the full Api host/DI composition (Milestone M6) wired up yet.
/// </summary>
public sealed class BillingDbContextFactory : IDesignTimeDbContextFactory<BillingDbContext>
{
    public BillingDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=jobtracker;Username=postgres;Password=postgres";

        DbContextOptions<BillingDbContext> options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new BillingDbContext(options);
    }
}
