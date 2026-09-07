using JobTracker.Api.Common;
using JobTracker.Modules.Jobs.Application.Commands.CompleteJob;
using JobTracker.Modules.Jobs.Application.Commands.CreateJob;
using JobTracker.Modules.Jobs.Application.Queries.SearchJobs;
using JobTracker.SharedKernel.Multitenancy;
using MediatR;

namespace JobTracker.Api.Endpoints.Jobs;

public static class JobsEndpoints
{
    public static IEndpointRouteBuilder MapJobsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/jobs").WithTags("Jobs");

        group.MapPost("/", CreateJobAsync);
        group.MapPost("/{id:guid}/complete", CompleteJobAsync);
        group.MapGet("/", SearchJobsAsync);

        return app;
    }

    private static async Task<IResult> CreateJobAsync(
        CreateJobRequest request,
        ICurrentOrganizationProvider tenant,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateJobCommand(
            request.Title,
            request.Description,
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Latitude,
            request.Longitude,
            request.CustomerId,
            tenant.OrganizationId,
            request.Notes,
            request.ScheduledDate,
            request.AssigneeId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult(id => Results.Created($"/jobs/{id}", new { id }));
    }

    private static async Task<IResult> CompleteJobAsync(
        Guid id,
        CompleteJobRequest request,
        ICurrentOrganizationProvider tenant,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CompleteJobCommand(id, tenant.OrganizationId, request.SignatureUrl);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult> SearchJobsAsync(
        [AsParameters] SearchJobsRequest request,
        ICurrentOrganizationProvider tenant,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new SearchJobsQuery(
            tenant.OrganizationId,
            request.SearchTerm,
            request.Statuses,
            request.FromDate,
            request.ToDate,
            request.AssigneeId,
            request.Page ?? 1,
            request.PageSize ?? 20);

        var result = await sender.Send(query, cancellationToken);

        return result.ToHttpResult();
    }
}
