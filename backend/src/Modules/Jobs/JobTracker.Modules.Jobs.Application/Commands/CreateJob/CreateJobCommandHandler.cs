using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Application;
using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.Commands.CreateJob;

internal sealed class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<Guid>>
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        Result<Address> addressResult = Address.Create(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Latitude,
            request.Longitude);

        if (addressResult.IsFailure)
        {
            return Result.Failure<Guid>(addressResult.Error);
        }

        DateTime utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        Result<Job> jobResult = Job.Create(
            request.Title,
            request.Description,
            addressResult.Value,
            request.CustomerId,
            request.OrganizationId,
            utcNow,
            request.Notes,
            request.ScheduledDate,
            request.AssigneeId);

        if (jobResult.IsFailure)
        {
            return Result.Failure<Guid>(jobResult.Error);
        }

        await _jobRepository.AddAsync(jobResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return jobResult.Value.Id;
    }
}
