using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.Commands.CompleteJob;

/// <summary>
/// Returns the non-generic <see cref="Result"/> rather than MediatR's <c>Unit</c> —
/// our Result type already models "success without a value", so Result&lt;Unit&gt; would
/// be redundant.
/// </summary>
public sealed record CompleteJobCommand(
    Guid JobId,
    Guid OrganizationId,
    string SignatureUrl) : IRequest<Result>;
