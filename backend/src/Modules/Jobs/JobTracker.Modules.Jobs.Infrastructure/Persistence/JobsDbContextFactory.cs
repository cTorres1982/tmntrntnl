using EFCore.NamingConventions;
using JobTracker.SharedKernel.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` construct JobsDbContext at design time — a plain
/// environment variable rather than the Api host's appsettings/user-secrets, since
/// this Infrastructure project has no access to (and shouldn't depend on) the Api
/// project's configuration setup. No hardcoded fallback: a missing connection
/// string fails loudly rather than silently guessing a password.
/// </summary>
public sealed class JobsDbContextFactory : IDesignTimeDbContextFactory<JobsDbContext>
{
    public JobsDbContext CreateDbContext(string[] args)
    {
        string connectionString = Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Set the JOBTRACKER_CONNECTION_STRING environment variable before running `dotnet ef` commands.");

        DbContextOptions<JobsDbContext> options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new JobsDbContext(options, new NullCurrentOrganizationProvider());
    }
}
