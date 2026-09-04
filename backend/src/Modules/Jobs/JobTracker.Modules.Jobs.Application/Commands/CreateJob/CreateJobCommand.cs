using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.Commands.CreateJob;

public sealed record CreateJobCommand(
    string Title,
    string Description,
    string Street,
    string City,
    string State,
    string ZipCode,
    double Latitude,
    double Longitude,
    Guid CustomerId,
    Guid OrganizationId,
    string? Notes,
    DateTime? ScheduledDate,
    Guid? AssigneeId) : IRequest<Result<Guid>>;
