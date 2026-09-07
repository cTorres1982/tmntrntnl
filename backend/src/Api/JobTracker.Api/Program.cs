using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using JobTracker.Api.Endpoints.Jobs;
using JobTracker.Api.Multitenancy;
using JobTracker.Modules.Billing.Application;
using JobTracker.Modules.Billing.Infrastructure;
using JobTracker.Modules.Jobs.Application;
using JobTracker.Modules.Jobs.Infrastructure;
using JobTracker.Modules.Jobs.Infrastructure.Outbox;
using JobTracker.SharedKernel.Multitenancy;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// System.Text.Json serializes enums as their underlying int by default —
// independent of EF Core's HasConversion<string>(), which only governs
// database storage. Without this, JobResponse.Status (and every other enum
// in an API response) would go over the wire as a number, breaking every
// frontend consumer that expects the string literal (e.g. "Draft") the
// domain and database both already use.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

string connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "No Postgres connection string configured. Set ConnectionStrings:Postgres " +
        "(via `dotnet user-secrets` for local dev) or the JOBTRACKER_CONNECTION_STRING " +
        "environment variable.");

builder.Services.AddSingleton(TimeProvider.System);

// Registered once here, not inside each module's own DependencyInjection — every
// module's DbContext (and the outbox dispatcher) needs the SAME scoped instance.
builder.Services.AddScoped<CurrentOrganizationProvider>();
builder.Services.AddScoped<ICurrentOrganizationProvider>(sp => sp.GetRequiredService<CurrentOrganizationProvider>());

builder.Services.AddJobsApplication();
builder.Services.AddJobsInfrastructure(connectionString, builder.Configuration);

builder.Services.AddBillingApplication();
builder.Services.AddBillingInfrastructure(connectionString);

builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(
        options => options.UseNpgsqlConnection(connectionString),
        new PostgreSqlStorageOptions { SchemaName = "hangfire" }));
builder.Services.AddHangfireServer();

WebApplication app = builder.Build();

// Dev-mode tenant resolution (X-Organization-Id header) — see
// TenantContextMiddleware for why this stands in for real auth.
app.UseMiddleware<TenantContextMiddleware>();

app.MapJobsEndpoints();

// Dev-only: no auth on the dashboard. A real deployment would restrict this
// (Hangfire's DashboardOptions.Authorization) before ever being reachable.
app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ProcessOutboxMessagesJob>(
    "process-jobs-outbox",
    job => job.ExecuteAsync(CancellationToken.None),
    "*/10 * * * * *");

app.Run();
