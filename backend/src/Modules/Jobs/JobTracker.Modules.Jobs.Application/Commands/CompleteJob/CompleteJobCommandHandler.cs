using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.Commands.CompleteJob;

internal sealed class CompleteJobCommandHandler : IRequestHandler<CompleteJobCommand, Result>
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CompleteJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(CompleteJobCommand request, CancellationToken cancellationToken)
    {
        Job? job = await _jobRepository.GetByIdAsync(request.JobId, request.OrganizationId, cancellationToken);

        if (job is null)
        {
            return Result.Failure(JobErrors.NotFound);
        }

        Result completeResult = job.Complete(_timeProvider.GetUtcNow().UtcDateTime, request.SignatureUrl);

        if (completeResult.IsFailure)
        {
            return completeResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
