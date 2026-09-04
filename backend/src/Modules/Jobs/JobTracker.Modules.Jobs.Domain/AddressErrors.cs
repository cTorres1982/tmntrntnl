using JobTracker.SharedKernel.Results;

namespace JobTracker.Modules.Jobs.Domain;

public static class AddressErrors
{
    public static readonly Error StreetRequired = Error.Validation("Address.StreetRequired", "Street is required.");
    public static readonly Error CityRequired = Error.Validation("Address.CityRequired", "City is required.");
    public static readonly Error StateRequired = Error.Validation("Address.StateRequired", "State is required.");
    public static readonly Error ZipCodeRequired = Error.Validation("Address.ZipCodeRequired", "Zip code is required.");
    public static readonly Error InvalidLatitude = Error.Validation("Address.InvalidLatitude", "Latitude must be between -90 and 90.");
    public static readonly Error InvalidLongitude = Error.Validation("Address.InvalidLongitude", "Longitude must be between -180 and 180.");
}
