using JobTracker.SharedKernel.Results;

namespace JobTracker.Modules.Jobs.Domain;

public static class JobErrors
{
    public static readonly Error NotFound = Error.NotFound("Job.NotFound", "The job was not found.");
    public static readonly Error TitleRequired = Error.Validation("Job.TitleRequired", "Job title is required.");
    public static readonly Error ScheduledInPast = Error.Validation("Job.ScheduledInPast", "A job cannot be scheduled in the past.");
    public static readonly Error AssigneeRequired = Error.Validation("Job.AssigneeRequired", "An assignee is required to schedule a job.");
    public static readonly Error TerminalState = Error.Conflict("Job.TerminalState", "Completed or cancelled jobs cannot transition to any other state.");
    public static readonly Error InvalidTransition = Error.Conflict("Job.InvalidTransition", "Only scheduled jobs can move to in-progress.");
    public static readonly Error SignatureRequired = Error.Validation("Job.SignatureRequired", "A signature is required to complete a job.");
    public static readonly Error CancellationReasonRequired = Error.Validation("Job.CancellationReasonRequired", "A cancellation reason is required.");
    public static readonly Error PhotoUrlRequired = Error.Validation("Job.PhotoUrlRequired", "Photo URL is required.");
}
