using FluentValidation;

namespace JobTracker.Modules.Jobs.Application.Commands.CompleteJob;

internal sealed class CompleteJobCommandValidator : AbstractValidator<CompleteJobCommand>
{
    public CompleteJobCommandValidator()
    {
        RuleFor(c => c.JobId).NotEmpty();
        RuleFor(c => c.OrganizationId).NotEmpty();
        RuleFor(c => c.SignatureUrl).NotEmpty();
    }
}
