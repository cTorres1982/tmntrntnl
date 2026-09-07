using FluentAssertions;
using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Application.Commands.CompleteJob;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Domain.Events;
using Moq;
using Xunit;

namespace JobTracker.Modules.Jobs.Application.Tests.Commands.CompleteJob;

public class CompleteJobCommandHandlerTests
{
    private static readonly DateTime UtcNow = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Address ValidAddress =
        Address.Create("123 Main St", "Austin", "TX", "78701", 30.2672, -97.7431).Value;

    private readonly Mock<IJobRepository> _jobRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CompleteJobCommandHandler _sut;

    public CompleteJobCommandHandlerTests()
    {
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(t => t.GetUtcNow()).Returns(UtcNow);

        _sut = new CompleteJobCommandHandler(_jobRepositoryMock.Object, _unitOfWorkMock.Object, timeProvider.Object);
    }

    private static Job CreateInProgressJob()
    {
        var job = Job.Create("Roof replacement", "desc", ValidAddress, Guid.NewGuid(), Guid.NewGuid(), UtcNow.AddDays(-2)).Value;
        job.Schedule(UtcNow.AddDays(-1), Guid.NewGuid(), UtcNow.AddDays(-2));
        job.Start(UtcNow.AddDays(-1));
        job.ClearDomainEvents();
        return job;
    }

    [Fact]
    public async Task Handle_WhenJobNotFound_ReturnsNotFoundFailureWithoutSaving()
    {
        _jobRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Job?)null);

        var command = new CompleteJobCommand(Guid.NewGuid(), Guid.NewGuid(), "https://example.com/signature.png");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.NotFound);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenJobIsNotInProgress_ReturnsInvalidTransitionFailureWithoutSaving()
    {
        var draftJob = Job.Create("Roof replacement", "desc", ValidAddress, Guid.NewGuid(), Guid.NewGuid(), UtcNow).Value;

        _jobRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(draftJob);

        var command = new CompleteJobCommand(draftJob.Id, Guid.NewGuid(), "https://example.com/signature.png");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(JobErrors.InvalidTransition);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenJobIsInProgress_CompletesJobRaisesDomainEventAndSaves()
    {
        Job job = CreateInProgressJob();

        _jobRepositoryMock
            .Setup(repo => repo.GetByIdAsync(job.Id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        var command = new CompleteJobCommand(job.Id, Guid.NewGuid(), "https://example.com/signature.png");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        job.Status.Should().Be(JobStatus.Completed);
        job.DomainEvents.Should().ContainSingle(e => e is JobCompletedDomainEvent);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
