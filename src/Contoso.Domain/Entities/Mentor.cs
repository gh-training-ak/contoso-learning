using Contoso.Domain.Common;
using Contoso.Domain.ValueObjects;

namespace Contoso.Domain.Entities;

public sealed class Mentor : Entity
{
    private readonly List<Review> _reviews = [];
    private readonly List<Announcement> _announcements = [];

    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Biography { get; set; }
    public Address? Address { get; set; }
    public Money HourlyRate { get; set; } = Money.Zero;
    public bool AcceptsOnline { get; set; } = true;
    public bool AcceptsInPerson { get; set; }

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<Announcement> Announcements => _announcements.AsReadOnly();

    public decimal AverageRating =>
        _reviews.Count == 0 ? 0m : Math.Round(_reviews.Average(r => (decimal)r.Score), 2);

    public void AddReview(Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        if (review.SubjectId != Id)
        {
            throw new DomainException("That review belongs to a different mentor.");
        }

        _reviews.Add(review);
    }

    public void Publish(Announcement announcement)
    {
        ArgumentNullException.ThrowIfNull(announcement);
        _announcements.Add(announcement);
    }
}
