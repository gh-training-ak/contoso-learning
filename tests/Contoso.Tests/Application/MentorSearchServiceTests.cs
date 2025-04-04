using Contoso.Application.Abstractions;
using Contoso.Application.Mentors;
using Contoso.Domain.Entities;
using Contoso.Domain.ValueObjects;
using Contoso.Infrastructure.Repositories;

namespace Contoso.Tests.Application;

public sealed class MentorSearchServiceTests
{
    private static (MentorSearchService Service, InMemoryMentorRepository Repo) Build()
    {
        var repo = new InMemoryMentorRepository();
        return (new MentorSearchService(repo), repo);
    }

    private static Mentor Mentor(string name, decimal rate, params int[] scores)
    {
        var mentor = new Mentor
        {
            DisplayName = name,
            Email = $"{name.Replace(" ", ".").ToLowerInvariant()}@contoso.example",
            HourlyRate = new Money(rate)
        };

        foreach (var score in scores)
        {
            mentor.AddReview(Review.Create(Guid.NewGuid(), mentor.Id, score));
        }

        return mentor;
    }

    [Fact]
    public async Task EmptyRepositoryReturnsNoResults()
    {
        var (service, _) = Build();

        Assert.Empty(await service.SearchAsync(new MentorSearchCriteria()));
    }

    [Fact]
    public async Task ResultsAreOrderedByRatingDescending()
    {
        var (service, repo) = Build();
        repo.Seed(Mentor("Low Rated", 20m, 3, 3), Mentor("High Rated", 20m, 5, 5));

        var results = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Equal("High Rated", results[0].DisplayName);
    }

    [Fact]
    public async Task MaxHourlyRateFiltersResults()
    {
        var (service, repo) = Build();
        repo.Seed(Mentor("Cheap", 15m, 4), Mentor("Expensive", 80m, 5));

        var results = await service.SearchAsync(new MentorSearchCriteria { MaxHourlyRate = 20m });

        Assert.Single(results);
        Assert.Equal("Cheap", results[0].DisplayName);
    }

    [Fact]
    public async Task SoftDeletedMentorsAreExcluded()
    {
        var (service, repo) = Build();
        var gone = Mentor("Departed", 20m, 5);
        gone.SoftDelete(DateTimeOffset.UtcNow);
        repo.Seed(gone, Mentor("Present", 20m, 4));

        var results = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Single(results);
        Assert.Equal("Present", results[0].DisplayName);
    }
}
