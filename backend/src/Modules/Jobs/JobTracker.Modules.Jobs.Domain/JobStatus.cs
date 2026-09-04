namespace JobTracker.Modules.Jobs.Domain;

public enum JobStatus
{
    Draft = 0,
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
}
