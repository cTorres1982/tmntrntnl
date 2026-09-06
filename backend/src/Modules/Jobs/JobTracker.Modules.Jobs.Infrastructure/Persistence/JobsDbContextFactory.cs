using EFCore.NamingConventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` construct JobsDbContext at design time without needing
/// the full Api host/DI composition (Milestone M6) wired up yet.
/// </summary>
public sealed class JobsDbContextFactory : IDesignTimeDbContextFactory<JobsDbContext>
{
    public JobsDbContext CreateDbContext(string[] args)
    {
        //TODO - use a proper configuration provider to get the connection string instead of hardcoding it here
        string connectionString = Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=jobtracker;Username=postgres;Password=postgres";

        DbContextOptions<JobsDbContext> options = new DbContextOptionsBuilder<JobsDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new JobsDbContext(options);
    }
}
