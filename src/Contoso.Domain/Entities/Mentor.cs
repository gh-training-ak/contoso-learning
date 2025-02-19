using Contoso.Domain.Common;
using Contoso.Domain.ValueObjects;

namespace Contoso.Domain.Entities;

public sealed class Mentor : Entity
{
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Biography { get; set; }
    public Address? Address { get; set; }
    public decimal HourlyRate { get; set; }
    public bool AcceptsOnline { get; set; } = true;
    public bool AcceptsInPerson { get; set; }

}
