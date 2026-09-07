using EFCore.NamingConventions;
using JobTracker.SharedKernel.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` construct BillingDbContext at design time — a plain
/// environment variable rather than the Api host's appsettings/user-secrets, since
/// this Infrastructure project has no access to (and shouldn't depend on) the Api
/// project's configuration setup. No hardcoded fallback: a missing connection
/// string fails loudly rather than silently guessing a password.
/// </summary>
public sealed class BillingDbContextFactory : IDesignTimeDbContextFactory<BillingDbContext>
{
    public BillingDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Set the JOBTRACKER_CONNECTION_STRING environment variable before running `dotnet ef` commands.");

        DbContextOptions<BillingDbContext> options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new BillingDbContext(options, new NullCurrentOrganizationProvider());
    }
}
