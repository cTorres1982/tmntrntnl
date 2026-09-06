using EFCore.NamingConventions;
using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Infrastructure.Notifications;
using JobTracker.Modules.Jobs.Infrastructure.Outbox;
using JobTracker.Modules.Jobs.Infrastructure.Persistence;
using JobTracker.Modules.Jobs.Infrastructure.Persistence.Repositories;
using JobTracker.SharedKernel.Application;
using JobTracker.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;

namespace JobTracker.Modules.Jobs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJobsInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        services.AddDbContext<JobsDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "jobs"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new InsertOutboxMessagesInterceptor()));

        services.AddScoped<IJobsDbContext>(sp => sp.GetRequiredService<JobsDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<JobsDbContext>());
        services.AddScoped<IJobRepository, JobRepository>();

        services.Configure<SendGridOptions>(configuration.GetSection(SendGridOptions.SectionName));

        SendGridOptions sendGridOptions = configuration.GetSection(SendGridOptions.SectionName).Get<SendGridOptions>()
            ?? new SendGridOptions();

        if (!string.IsNullOrWhiteSpace(sendGridOptions.ApiKey))
        {
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridOptions.ApiKey));
            services.AddScoped<INotificationService, SendGridNotificationService>();
        }
        else
        {
            services.AddScoped<INotificationService, ConsoleNotificationService>();
        }

        services.AddScoped<ProcessOutboxMessagesJob>();

        return services;
    }
}
