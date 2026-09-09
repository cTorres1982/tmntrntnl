using FluentAssertions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Domain.Events;
using Xunit;

namespace JobTracker.Modules.Jobs.Domain.Tests;

public class JobTests
{
    private static readonly DateTime UtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Address ValidAddress = Address.Create("123 Main St", "Austin", "TX", "78701", 30.2672, -97.7431).Value;

    private static JobTracker.SharedKernel.Results.Result<Job> CreateDraft() =>
        Job.Create("Roof replacement", "Full tear-off and replace", ValidAddress, Guid.NewGuid(), Guid.NewGuid(), UtcNow);

    [Fact]
    public void Create_WithValidData_ReturnsDraftJobAndRaisesJobCreatedDomainEvent()
    {
        var result = CreateDraft();

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(JobStatus.Draft);
        result.Value.DomainEvents.Should().ContainSingle(e => e is JobCreatedDomainEvent);
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsFailure()
    {
        var result = Job.Create(string.Empty, "desc", ValidAddress, Guid.NewGuid(), Guid.NewGuid(), UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.TitleRequired);
    }

    [Fact]
    public void Create_WithPastScheduledDate_ReturnsFailure()
    {
        var result = Job.Create(
            "Roof replacement",
            "desc",
            ValidAddress,
            Guid.NewGuid(),
            Guid.NewGuid(),
            UtcNow,
            scheduledDate: UtcNow.AddDays(-1),
            assigneeId: Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.ScheduledInPast);
    }

    [Fact]
    public void Schedule_WithPastDate_ReturnsFailure()
    {
        var job = CreateDraft().Value;

        var result = job.Schedule(UtcNow.AddDays(-1), Guid.NewGuid(), UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.ScheduledInPast);
        job.Status.Should().Be(JobStatus.Draft);
    }

    [Fact]
    public void Schedule_WithoutAssignee_ReturnsFailure()
    {
        var job = CreateDraft().Value;

        var result = job.Schedule(UtcNow.AddDays(1), null, UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.AssigneeRequired);
    }

    [Fact]
    public void Schedule_WithValidFutureDateAndAssignee_TransitionsToScheduled()
    {
        var job = CreateDraft().Value;
        var assigneeId = Guid.NewGuid();

        var result = job.Schedule(UtcNow.AddDays(1), assigneeId, UtcNow);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Scheduled);
        job.AssigneeId.Should().Be(assigneeId);
    }

    [Fact]
    public void Schedule_ForLaterTheSameCalendarDay_Succeeds()
    {
        // scheduledDate comes from a date-only picker (no time-of-day), so
        // "today" is a valid choice even if its midnight instant is earlier
        // than the exact current time — comparing full timestamps instead of
        // just the calendar date would incorrectly reject this (and, near the
        // UTC day boundary, sometimes reject "tomorrow" too).
        var job = CreateDraft().Value;
        var assigneeId = Guid.NewGuid();
        DateTime laterToday = UtcNow.Date;

        var result = job.Schedule(laterToday, assigneeId, UtcNow);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Scheduled);
    }

    [Fact]
    public void Start_WhenDraft_ReturnsInvalidTransitionFailure()
    {
        var job = CreateDraft().Value;

        var result = job.Start(UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.InvalidTransition);
    }

    [Fact]
    public void Start_WhenScheduled_TransitionsToInProgress()
    {
        var job = CreateDraft().Value;
        job.Schedule(UtcNow.AddDays(1), Guid.NewGuid(), UtcNow);

        var result = job.Start(UtcNow.AddDays(1));

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.InProgress);
    }

    [Fact]
    public void Complete_WhenNotInProgress_ReturnsInvalidTransitionFailure()
    {
        var job = CreateDraft().Value;

        var result = job.Complete(UtcNow, "https://example.com/signature.png");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.InvalidTransition);
    }

    [Fact]
    public void Complete_WhenInProgress_TransitionsToCompletedAndRaisesDomainEvent()
    {
        var job = CreateDraft().Value;
        job.Schedule(UtcNow.AddDays(1), Guid.NewGuid(), UtcNow);
        job.Start(UtcNow.AddDays(1));

        var result = job.Complete(UtcNow.AddDays(2), "https://example.com/signature.png");

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Completed);
        job.DomainEvents.Should().ContainSingle(e => e is JobCompletedDomainEvent);
    }

    [Theory]
    [MemberData(nameof(TerminalJobs))]
    public void AnyTransition_WhenJobIsInTerminalState_ReturnsTerminalStateFailure(Job job)
    {
        job.Schedule(UtcNow.AddDays(1), Guid.NewGuid(), UtcNow).IsFailure.Should().BeTrue();
        job.Start(UtcNow).IsFailure.Should().BeTrue();
        job.Complete(UtcNow, "https://example.com/signature.png").IsFailure.Should().BeTrue();
        job.Cancel("changed my mind", UtcNow).IsFailure.Should().BeTrue();
        job.AddPhoto("https://example.com/photo.png", UtcNow, null).IsFailure.Should().BeTrue();
    }

    public static IEnumerable<object[]> TerminalJobs()
    {
        var completed = CreateDraft().Value;
        completed.Schedule(UtcNow.AddDays(1), Guid.NewGuid(), UtcNow);
        completed.Start(UtcNow.AddDays(1));
        completed.Complete(UtcNow.AddDays(2), "https://example.com/signature.png");
        yield return new object[] { completed };

        var cancelled = CreateDraft().Value;
        cancelled.Cancel("customer request", UtcNow);
        yield return new object[] { cancelled };
    }

    [Fact]
    public void Cancel_WhenValid_TransitionsToCancelledAndRaisesDomainEvent()
    {
        var job = CreateDraft().Value;

        var result = job.Cancel("customer request", UtcNow);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Cancelled);
        job.DomainEvents.Should().ContainSingle(e => e is JobCancelledDomainEvent);
    }

    [Fact]
    public void AddPhoto_WhenValid_IsOnlyReachableThroughAggregateAndAddsToReadOnlyCollection()
    {
        var job = CreateDraft().Value;

        var result = job.AddPhoto("https://example.com/photo.png", UtcNow, "front of house");

        result.IsSuccess.Should().BeTrue();
        job.Photos.Should().ContainSingle(p => p.Url == "https://example.com/photo.png");
    }
}
