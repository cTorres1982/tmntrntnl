using NetArchTest.Rules;
using Xunit;

namespace JobTracker.ArchitectureTests;

/// <summary>
/// Enforces the dependency direction Clean Architecture requires (Domain knows
/// nothing about Application/Infrastructure; Application knows nothing about
/// Infrastructure) and bounded context isolation (Jobs never references Billing —
/// the two modules only ever meet through JobTracker.Modules.Jobs.IntegrationEvents,
/// the public contract Billing depends on instead).
/// </summary>
public class LayeringTests
{
    [Fact]
    public void JobsDomain_ShouldNotDependOnApplicationOrInfrastructure()
    {
        Types.InAssembly(typeof(JobTracker.Modules.Jobs.Domain.Job).Assembly)
            .Should().NotHaveDependencyOnAny(
                "JobTracker.Modules.Jobs.Application",
                "JobTracker.Modules.Jobs.Infrastructure")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void JobsApplication_ShouldNotDependOnInfrastructure()
    {
        Types.InAssembly(typeof(JobTracker.Modules.Jobs.Application.DependencyInjection).Assembly)
            .Should().NotHaveDependencyOn("JobTracker.Modules.Jobs.Infrastructure")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void BillingDomain_ShouldNotDependOnApplicationOrInfrastructure()
    {
        Types.InAssembly(typeof(JobTracker.Modules.Billing.Domain.Invoice).Assembly)
            .Should().NotHaveDependencyOnAny(
                "JobTracker.Modules.Billing.Application",
                "JobTracker.Modules.Billing.Infrastructure")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void BillingApplication_ShouldNotDependOnInfrastructure()
    {
        Types.InAssembly(typeof(JobTracker.Modules.Billing.Application.DependencyInjection).Assembly)
            .Should().NotHaveDependencyOn("JobTracker.Modules.Billing.Infrastructure")
            .GetResult()
            .ShouldBeSuccessful();
    }

    [Fact]
    public void JobsModule_ShouldNeverDependOnBillingModule()
    {
        System.Reflection.Assembly[] jobsAssemblies =
        [
            typeof(JobTracker.Modules.Jobs.Domain.Job).Assembly,
            typeof(JobTracker.Modules.Jobs.Application.DependencyInjection).Assembly,
            typeof(JobTracker.Modules.Jobs.Infrastructure.DependencyInjection).Assembly,
            typeof(JobTracker.Modules.Jobs.IntegrationEvents.JobCompletedIntegrationEvent).Assembly,
        ];

        foreach (System.Reflection.Assembly assembly in jobsAssemblies)
        {
            Types.InAssembly(assembly)
                .Should().NotHaveDependencyOn("JobTracker.Modules.Billing")
                .GetResult()
                .ShouldBeSuccessful();
        }
    }
}
