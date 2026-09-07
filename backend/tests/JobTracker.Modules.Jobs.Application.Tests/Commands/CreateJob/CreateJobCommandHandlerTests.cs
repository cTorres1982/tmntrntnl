using FluentAssertions;
using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Application.Commands.CreateJob;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Domain.Events;
using Moq;
using Xunit;

namespace JobTracker.Modules.Jobs.Application.Tests.Commands.CreateJob;

public class CreateJobCommandHandlerTests
{
    private static readonly DateTime UtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IJobRepository> _jobRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateJobCommandHandler _sut;

    public CreateJobCommandHandlerTests()
    {
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(t => t.GetUtcNow()).Returns(UtcNow);

        _sut = new CreateJobCommandHandler(_jobRepositoryMock.Object, _unitOfWorkMock.Object, timeProvider.Object);
    }

    private static CreateJobCommand ValidCommand(DateTime? scheduledDate = null, Guid? assigneeId = null) =>
        new(
            "Roof replacement",
            "Full tear-off and replace",
            "123 Main St",
            "Austin",
            "TX",
            "78701",
            30.2672,
            -97.7431,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Notes: null,
            ScheduledDate: scheduledDate,
            AssigneeId: assigneeId);

    [Fact]
    public async Task Handle_WithValidCommand_PersistsJobAndRaisesJobCreatedDomainEvent()
    {
        Job? capturedJob = null;
        _jobRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Callback<Job, CancellationToken>((job, _) => capturedJob = job)
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        capturedJob.Should().NotBeNull();
        capturedJob!.DomainEvents.Should().ContainSingle(e => e is JobCreatedDomainEvent);
        result.Value.Should().Be(capturedJob.Id);

        _jobRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidAddress_ReturnsFailureWithoutPersisting()
    {
        var command = ValidCommand() with { Latitude = 999 };

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AddressErrors.InvalidLatitude);

        _jobRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithPastScheduledDate_ReturnsFailureWithoutPersisting()
    {
        var command = ValidCommand(scheduledDate: UtcNow.AddDays(-1), assigneeId: Guid.NewGuid());

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.ScheduledInPast);

        _jobRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
