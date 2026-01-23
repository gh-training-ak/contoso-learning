using Contoso.Application.Abstractions;
using Contoso.Application.Mentors;
using Contoso.Domain.Entities;
using Contoso.Domain.ValueObjects;
using Contoso.Infrastructure.Caching;
using Contoso.Infrastructure.Repositories;

namespace Contoso.Tests.Application;

public sealed class MentorSearchServiceTests
{
    private static (MentorSearchService Service, InMemoryMentorRepository Repo, InMemoryCacheStore Cache) Build()
    {
        var repo = new InMemoryMentorRepository();
        var cache = new InMemoryCacheStore(TimeProvider.System);
        return (new MentorSearchService(repo, cache), repo, cache);
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
    public async Task EmptyRepositoryReturnsEmptyPage()
    {
        var (service, _, _) = Build();

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task ResultsAreOrderedByRatingDescending()
    {
        var (service, repo, _) = Build();
        repo.Seed(Mentor("Low Rated", 20m, 3, 3), Mentor("High Rated", 20m, 5, 5));

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Equal("High Rated", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task MaxHourlyRateFiltersResults()
    {
        var (service, repo, _) = Build();
        repo.Seed(Mentor("Cheap", 15m, 4), Mentor("Expensive", 80m, 5));

        var result = await service.SearchAsync(new MentorSearchCriteria { MaxHourlyRate = 20m });

        Assert.Single(result.Items);
        Assert.Equal("Cheap", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task SecondCallIsServedFromCache()
    {
        var (service, repo, cache) = Build();
        repo.Seed(Mentor("Cached Mentor", 20m, 5));
        var criteria = new MentorSearchCriteria();

        await service.SearchAsync(criteria);
        var cachedBefore = cache.Count;
        await service.SearchAsync(criteria);

        Assert.Equal(1, cachedBefore);
        Assert.Equal(1, cache.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public async Task InvalidPageSizeThrows(int pageSize)
    {
        var (service, _, _) = Build();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(new MentorSearchCriteria { PageSize = pageSize }));
    }

    [Fact]
    public async Task SoftDeletedMentorsAreExcluded()
    {
        var (service, repo, _) = Build();
        var gone = Mentor("Departed", 20m, 5);
        gone.SoftDelete(DateTimeOffset.UtcNow);
        repo.Seed(gone, Mentor("Present", 20m, 4));

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Single(result.Items);
        Assert.Equal("Present", result.Items[0].DisplayName);
    }
}
