using Contoso.Domain.ValueObjects;

namespace Contoso.Tests.Domain;

public sealed class AddressTests
{
    private static readonly Address CityHall = new("Donegall Square", "Belfast", "BT1 5GS", 54.5967, -5.9301);
    private static readonly Address Botanic = new("Botanic Avenue", "Belfast", "BT7 1JQ", 54.5826, -5.9335);
    private static readonly Address Dublin = new("O'Connell Street", "Dublin", "D01", 53.3498, -6.2603);

    [Fact]
    public void DistanceToItselfIsZero()
    {
        Assert.True(CityHall.DistanceKmTo(CityHall) < 0.001);
    }

    [Fact]
    public void DistanceIsSymmetric()
    {
        var there = CityHall.DistanceKmTo(Dublin);
        var back = Dublin.DistanceKmTo(CityHall);

        Assert.True(Math.Abs(there - back) < 0.001);
    }

