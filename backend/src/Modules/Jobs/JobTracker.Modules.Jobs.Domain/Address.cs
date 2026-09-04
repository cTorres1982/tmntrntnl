using JobTracker.SharedKernel.Domain;
using JobTracker.SharedKernel.Results;

namespace JobTracker.Modules.Jobs.Domain;

/// <summary>
/// Value Object: equality is structural, never by reference. Mapped as an
/// EF Core owned type by the Infrastructure layer (no identity of its own).
/// </summary>
public sealed class Address : ValueObject
{
    private Address(string street, string city, string state, string zipCode, double latitude, double longitude)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Latitude = latitude;
        Longitude = longitude;
    }

    private Address()
    {
    }

    public string Street { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    public string ZipCode { get; private set; } = string.Empty;

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public static Result<Address> Create(
        string street,
        string city,
        string state,
        string zipCode,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<Address>(AddressErrors.StreetRequired);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result.Failure<Address>(AddressErrors.CityRequired);
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return Result.Failure<Address>(AddressErrors.StateRequired);
        }

        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Result.Failure<Address>(AddressErrors.ZipCodeRequired);
        }

        if (latitude is < -90 or > 90)
        {
            return Result.Failure<Address>(AddressErrors.InvalidLatitude);
        }

        if (longitude is < -180 or > 180)
        {
            return Result.Failure<Address>(AddressErrors.InvalidLongitude);
        }

        return new Address(street, city, state, zipCode, latitude, longitude);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Latitude;
        yield return Longitude;
    }
}
