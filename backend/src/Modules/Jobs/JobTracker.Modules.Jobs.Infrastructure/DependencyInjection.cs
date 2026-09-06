using EFCore.NamingConventions;
using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Infrastructure.Persistence;
using JobTracker.Modules.Jobs.Infrastructure.Persistence.Repositories;
using JobTracker.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Modules.Jobs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<JobsDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "jobs"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new InsertOutboxMessagesInterceptor()));

        services.AddScoped<IJobsDbContext>(sp => sp.GetRequiredService<JobsDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<JobsDbContext>());
        services.AddScoped<IJobRepository, JobRepository>();

        return services;
    }
}
