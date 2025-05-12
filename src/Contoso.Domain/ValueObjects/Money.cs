using Contoso.Domain.Common;

namespace Contoso.Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency = "GBP")
{
    public static Money Zero => new(0m);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return this with { Amount = Amount + other.Amount };
    }

    public Money Multiply(decimal factor) => this with { Amount = Math.Round(Amount * factor, 2) };

    private void EnsureSameCurrency(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException($"Cannot combine {Currency} with {other.Currency}.");
        }
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
