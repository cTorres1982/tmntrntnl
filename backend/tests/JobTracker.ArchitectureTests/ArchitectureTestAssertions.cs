using FluentAssertions;
using NetArchTest.Rules;

namespace JobTracker.ArchitectureTests;

internal static class ArchitectureTestAssertions
{
    public static void ShouldBeSuccessful(this TestResult result)
    {
        IEnumerable<string> failingTypeNames = result.FailingTypes?.Select(t => t.FullName ?? t.Name)
            ?? Enumerable.Empty<string>();

        result.IsSuccessful.Should().BeTrue(
            "the following types violate the rule: {0}",
            string.Join(", ", failingTypeNames));
    }
}
