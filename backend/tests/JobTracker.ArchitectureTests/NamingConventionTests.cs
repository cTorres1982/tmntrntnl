using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace JobTracker.ArchitectureTests;

/// <summary>
/// Enforces AC.md 3.2.4's naming conventions across every module's Application
/// assembly: Commands/Queries sealed and named accordingly; Handlers/Validators
/// internal sealed and named accordingly.
/// </summary>
public class NamingConventionTests
{
    private static readonly Assembly[] ApplicationAssemblies =
    [
        typeof(JobTracker.Modules.Jobs.Application.DependencyInjection).Assembly,
        typeof(JobTracker.Modules.Billing.Application.DependencyInjection).Assembly,
    ];

    [Fact]
    public void Commands_ShouldBeSealedAndEndWithCommand()
    {
        foreach (Assembly assembly in ApplicationAssemblies)
        {
            Types.InAssembly(assembly)
                .That().HaveNameEndingWith("Command")
                .And().AreClasses()
                .Should().BeSealed()
                .GetResult()
                .ShouldBeSuccessful();
        }
    }

    [Fact]
    public void CommandHandlers_ShouldBeInternalSealedAndEndWithCommandHandler()
    {
        foreach (Assembly assembly in ApplicationAssemblies)
        {
            Types.InAssembly(assembly)
                .That().HaveNameEndingWith("CommandHandler")
                .Should().BeSealed().And().NotBePublic()
                .GetResult()
                .ShouldBeSuccessful();
        }
    }

    [Fact]
    public void Queries_ShouldBeSealedAndEndWithQuery()
    {
        foreach (Assembly assembly in ApplicationAssemblies)
        {
            Types.InAssembly(assembly)
                .That().HaveNameEndingWith("Query")
                .And().AreClasses()
                .Should().BeSealed()
                .GetResult()
                .ShouldBeSuccessful();
        }
    }

    [Fact]
    public void QueryHandlers_ShouldBeInternalSealedAndEndWithQueryHandler()
    {
        foreach (Assembly assembly in ApplicationAssemblies)
        {
            Types.InAssembly(assembly)
                .That().HaveNameEndingWith("QueryHandler")
                .Should().BeSealed().And().NotBePublic()
                .GetResult()
                .ShouldBeSuccessful();
        }
    }

    [Fact]
    public void Validators_ShouldBeInternalSealedAndEndWithValidator()
    {
        foreach (Assembly assembly in ApplicationAssemblies)
        {
            Types.InAssembly(assembly)
                .That().HaveNameEndingWith("Validator")
                .Should().BeSealed().And().NotBePublic()
                .GetResult()
                .ShouldBeSuccessful();
        }
    }
}
