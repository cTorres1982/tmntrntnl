using JobTracker.Modules.Jobs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Endpoints.Customers;

/// <summary>
/// Read-only lookup for the create-job form's customer picker (AC.md never
/// asks for customer management — this exists purely so a user can't type an
/// unknown GUID, not as a real "Customers" feature; see the tenant confirming
/// this stayed intentionally minimal). No Application/CQRS layer for the same
/// reason JobsDbContext.Customers is exposed directly: Customer isn't a real
/// domain concept in this bounded context.
/// </summary>
public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/customers", GetCustomersAsync).WithTags("Customers");

        return app;
    }

    private static async Task<IResult> GetCustomersAsync(JobsDbContext dbContext, CancellationToken cancellationToken)
    {
        List<CustomerSummary> customers = await dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.Name)
            .Select(customer => new CustomerSummary(customer.Id, customer.Name))
            .ToListAsync(cancellationToken);

        return Results.Ok(customers);
    }
}
