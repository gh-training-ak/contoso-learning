using Contoso.Domain.Common;
using Contoso.Domain.ValueObjects;

namespace Contoso.Domain.Entities;

public sealed class Student : Entity
{
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public Address? Address { get; set; }
    public int? SchoolYear { get; set; }
    public string? GuardianEmail { get; set; }

    public bool RequiresGuardianConsent => SchoolYear is not null && SchoolYear < 12;
}
