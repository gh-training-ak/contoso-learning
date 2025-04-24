using Contoso.Domain.Common;
using Contoso.Domain.ValueObjects;

namespace Contoso.Tests.Domain;

public sealed class MoneyTests
{
    [Fact]
    public void AddingSameCurrencyWorks()
    {
        var total = new Money(10.50m).Add(new Money(4.50m));

        Assert.Equal(15.00m, total.Amount);
    }

    [Fact]
    public void AddingDifferentCurrenciesThrows()
    {
        var gbp = new Money(10m, "GBP");
        var eur = new Money(10m, "EUR");

        var ex = Assert.Throws<DomainException>(() => gbp.Add(eur));
        Assert.Contains("GBP", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(10.00, 1.5, 15.00)]
    [InlineData(33.33, 3, 99.99)]
    [InlineData(0.005, 2, 0.01)]
    public void MultiplyRoundsToTwoDecimals(decimal amount, decimal factor, decimal expected)
    {
        Assert.Equal(expected, new Money(amount).Multiply(factor).Amount);
    }

