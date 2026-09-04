using FluentAssertions;
using JobTracker.Modules.Jobs.Domain;
using Xunit;

namespace JobTracker.Modules.Jobs.Domain.Tests;

public class AddressTests
{
    private static JobTracker.SharedKernel.Results.Result<Address> CreateValid() =>
        Address.Create("123 Main St", "Austin", "TX", "78701", 30.2672, -97.7431);

    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
        result.Value.City.Should().Be("Austin");
    }

    [Theory]
    [InlineData("", "Austin", "TX", "78701")]
    [InlineData("123 Main St", "", "TX", "78701")]
    [InlineData("123 Main St", "Austin", "", "78701")]
    [InlineData("123 Main St", "Austin", "TX", "")]
    public void Create_WithMissingRequiredField_ReturnsFailure(string street, string city, string state, string zip)
    {
        var result = Address.Create(street, city, state, zip, 30.2672, -97.7431);

        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    [InlineData(0, -181)]
    public void Create_WithOutOfRangeCoordinates_ReturnsFailure(double latitude, double longitude)
    {
        var result = Address.Create("123 Main St", "Austin", "TX", "78701", latitude, longitude);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TwoAddresses_WithSameComponents_AreStructurallyEqual()
    {
        var first = CreateValid().Value;
        var second = CreateValid().Value;

        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void TwoAddresses_WithDifferentComponents_AreNotEqual()
    {
        var first = CreateValid().Value;
        var second = Address.Create("456 Other Ave", "Austin", "TX", "78701", 30.2672, -97.7431).Value;

        first.Should().NotBe(second);
        (first != second).Should().BeTrue();
    }
}
