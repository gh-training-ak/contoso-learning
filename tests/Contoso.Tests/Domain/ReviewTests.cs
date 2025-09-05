using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Tests.Domain;

public sealed class ReviewTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void ScoreOutsideOneToFiveThrows(int score)
    {
        Assert.Throws<DomainException>(() => Review.Create(Guid.NewGuid(), Guid.NewGuid(), score));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void ValidScoreIsAccepted(int score)
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), score);

        Assert.Equal(score, review.Score);
    }
}
