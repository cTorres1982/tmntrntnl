using JobTracker.SharedKernel.Multitenancy;
using Microsoft.Extensions.Primitives;

namespace JobTracker.Api.Multitenancy;

/// <summary>
/// Dev-mode stand-in for real auth: reads the tenant from a header instead of a
/// validated JWT claim. Documented simplification — see README for how real
/// authentication would replace this (the claim would populate the same
/// CurrentOrganizationProvider, so nothing downstream would need to change).
/// </summary>
public sealed class TenantContextMiddleware
{
    private const string OrganizationHeaderName = "X-Organization-Id";

    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CurrentOrganizationProvider currentOrganizationProvider)
    {
        if (context.Request.Headers.TryGetValue(OrganizationHeaderName, out StringValues value)
            && Guid.TryParse(value, out Guid organizationId))
        {
            currentOrganizationProvider.OrganizationId = organizationId;
        }

        await _next(context);
    }
}
