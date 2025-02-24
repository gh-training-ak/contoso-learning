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
    public void SoftDeletedMentorIsNotActive()
    {
        var mentor = NewMentor();
        mentor.SoftDelete(DateTimeOffset.UtcNow);

        Assert.False(mentor.IsActive);
    }
}
