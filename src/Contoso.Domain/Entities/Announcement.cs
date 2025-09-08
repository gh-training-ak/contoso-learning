using Contoso.Domain.Common;
using Contoso.Domain.Enums;
using Contoso.Domain.ValueObjects;

namespace Contoso.Domain.Entities;

public sealed class Announcement : Entity
{
    public Guid MentorId { get; init; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public Subject Subject { get; set; }
    public MeetingType MeetingType { get; set; }
    public Money PricePerHour { get; set; } = Money.Zero;
    public bool IsPublished { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    public void Publish(DateTimeOffset when)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new DomainException("An announcement needs a title before it can be published.");
        }

        IsPublished = true;
        PublishedAt = when;
    }

    public void Unpublish()
    {
        IsPublished = false;
        PublishedAt = null;
    }
}
