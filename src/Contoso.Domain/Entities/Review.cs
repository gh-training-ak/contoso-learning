using Contoso.Domain.Common;

namespace Contoso.Domain.Entities;

public sealed class Review : Entity
{
    public const int MinimumScore = 1;
    public const int MaximumScore = 5;

    public Guid AuthorId { get; init; }
    public Guid SubjectId { get; init; }
    public int Score { get; init; }
    public string? Comment { get; set; }

    public static Review Create(Guid authorId, Guid subjectId, int score, string? comment = null)
    {
        if (score is < MinimumScore or > MaximumScore)
        {
            throw new DomainException($"Score must be between {MinimumScore} and {MaximumScore}.");
        }

        return new Review
        {
            AuthorId = authorId,
            SubjectId = subjectId,
            Score = score,
            Comment = comment
        };
    }
}
