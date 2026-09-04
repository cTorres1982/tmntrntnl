using FluentValidation;

namespace JobTracker.Modules.Jobs.Application.Queries.SearchJobs;

internal sealed class SearchJobsQueryValidator : AbstractValidator<SearchJobsQuery>
{
    public SearchJobsQueryValidator()
    {
        RuleFor(q => q.OrganizationId).NotEmpty();
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
        RuleFor(q => q.ToDate)
            .GreaterThanOrEqualTo(q => q.FromDate)
            .When(q => q.FromDate is not null && q.ToDate is not null)
            .WithMessage("ToDate must be on or after FromDate.");
    }
}
