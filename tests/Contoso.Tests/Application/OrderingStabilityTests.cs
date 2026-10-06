using Contoso.Application.Abstractions;
using Contoso.Application.Mentors;
using Contoso.Domain.Entities;
using Contoso.Domain.ValueObjects;
using Contoso.Infrastructure.Caching;
using Contoso.Infrastructure.Repositories;

namespace Contoso.Tests.Application;

public sealed class OrderingStabilityTests
{
    private static Mentor Rated(string name, params int[] scores)
    {
        var mentor = new Mentor
        {
            DisplayName = name,
            Email = $"{name.Replace(" ", ".").ToLowerInvariant()}@contoso.example",
            HourlyRate = new Money(30m)
        };

        foreach (var score in scores)
        {
            mentor.AddReview(Review.Create(Guid.NewGuid(), mentor.Id, score));
        }

        return mentor;
    }

    private static MentorSearchService Service(params Mentor[] mentors)
    {
        var repo = new InMemoryMentorRepository();
        repo.Seed(mentors);
        return new MentorSearchService(repo, new InMemoryCacheStore(TimeProvider.System));
    }

    [Fact]
    public async Task MentorsOnTheSameRatingKeepTheSameOrderAcrossCalls()
    {
        var service = Service(Rated("Alpha", 4, 5), Rated("Bravo", 4, 5), Rated("Charlie", 4, 5));

        var first = await service.SearchAsync(new MentorSearchCriteria { PageSize = 10 });
        var second = await service.SearchAsync(new MentorSearchCriteria { PageSize = 10, Page = 1 });

        Assert.Equal(
            first.Items.Select(m => m.DisplayName),
            second.Items.Select(m => m.DisplayName));
    }

    [Fact]
    public async Task EveryMentorAppearsExactlyOnceAcrossPages()
    {
        var mentors = Enumerable.Range(1, 9).Select(i => Rated($"Mentor {i:00}", 4, 5)).ToArray();
        var service = Service(mentors);

        var pageOne = await service.SearchAsync(new MentorSearchCriteria { Page = 1, PageSize = 5 });
        var pageTwo = await service.SearchAsync(new MentorSearchCriteria { Page = 2, PageSize = 5 });

        var seen = pageOne.Items.Concat(pageTwo.Items).Select(m => m.Id).ToList();

        Assert.Equal(seen.Count, seen.Distinct().Count());
    }
}
