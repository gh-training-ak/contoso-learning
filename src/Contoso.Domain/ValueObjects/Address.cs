namespace Contoso.Domain.ValueObjects;

public sealed record Address(string Line1, string City, string PostCode, double Latitude, double Longitude)
{
    private const double EarthRadiusKm = 6371.0;

    public double DistanceKmTo(Address other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude))
                * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return EarthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
