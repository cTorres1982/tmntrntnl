using EFCore.NamingConventions;
using JobTracker.Modules.Billing.Domain;
using JobTracker.Modules.Billing.Infrastructure.Persistence;
using JobTracker.Modules.Billing.Infrastructure.Persistence.Repositories;
using JobTracker.SharedKernel.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Modules.Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillingDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "billing"))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BillingDbContext>());
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        return services;
    }
}
