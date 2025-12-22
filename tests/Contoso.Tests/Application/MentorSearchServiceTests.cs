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
    public async Task EmptyRepositoryReturnsEmptyPage()
    {
        var (service, _) = Build();

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public async Task InvalidPageSizeThrows(int pageSize)
    {
        var (service, _) = Build();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.SearchAsync(new MentorSearchCriteria { PageSize = pageSize }));
    }

    [Fact]
    public async Task ResultsAreOrderedByRatingDescending()
    {
        var (service, repo) = Build();
        repo.Seed(Mentor("Low Rated", 20m, 3, 3), Mentor("High Rated", 20m, 5, 5));

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Equal("High Rated", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task MaxHourlyRateFiltersResults()
    {
        var (service, repo) = Build();
        repo.Seed(Mentor("Cheap", 15m, 4), Mentor("Expensive", 80m, 5));

        var result = await service.SearchAsync(new MentorSearchCriteria { MaxHourlyRate = 20m });

        Assert.Single(result.Items);
        Assert.Equal("Cheap", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task SoftDeletedMentorsAreExcluded()
    {
        var (service, repo) = Build();
        var gone = Mentor("Departed", 20m, 5);
        gone.SoftDelete(DateTimeOffset.UtcNow);
        repo.Seed(gone, Mentor("Present", 20m, 4));

        var result = await service.SearchAsync(new MentorSearchCriteria());

        Assert.Single(result.Items);
        Assert.Equal("Present", result.Items[0].DisplayName);
    }
}
