using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Tests.Domain;

public sealed class MentorTests
{
    private static Mentor NewMentor() => new()
    {
        DisplayName = "Test Mentor",
        Email = "test.mentor@contoso.example"
    };

    [Fact]
    public void MentorWithNoReviewsHasZeroRating()
    {
        Assert.Equal(0m, NewMentor().AverageRating);
    }

    [Fact]
    public void AverageRatingIsRoundedToTwoDecimals()
    {
        var mentor = NewMentor();
        mentor.AddReview(Review.Create(Guid.NewGuid(), mentor.Id, 5));
        mentor.AddReview(Review.Create(Guid.NewGuid(), mentor.Id, 4));
        mentor.AddReview(Review.Create(Guid.NewGuid(), mentor.Id, 4));

        Assert.Equal(4.33m, mentor.AverageRating);
    }

    [Fact]
    public void ReviewForAnotherMentorIsRejected()
    {
        var mentor = NewMentor();
        var strayReview = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 5);

        Assert.Throws<DomainException>(() => mentor.AddReview(strayReview));
    }

    [Fact]
    public void SoftDeletedMentorIsNotActive()
    {
        var mentor = NewMentor();
        mentor.SoftDelete(DateTimeOffset.UtcNow);

        Assert.False(mentor.IsActive);
    }
}
