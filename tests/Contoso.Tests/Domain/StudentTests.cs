using Contoso.Domain.Entities;

namespace Contoso.Tests.Domain;

public sealed class StudentTests
{
    private static Student Year(int? schoolYear) => new()
    {
        DisplayName = "Test Student",
        Email = "test.student@contoso.example",
        SchoolYear = schoolYear
    };

    [Theory]
    [InlineData(7)]
    [InlineData(9)]
    [InlineData(11)]
    public void StudentsBelowSixthFormNeedGuardianConsent(int schoolYear)
    {
        Assert.True(Year(schoolYear).RequiresGuardianConsent);
    }

    [Theory]
    [InlineData(12)]
    [InlineData(13)]
    public void SixthFormStudentsDoNot(int schoolYear)
    {
        Assert.False(Year(schoolYear).RequiresGuardianConsent);
    }

    [Fact]
    public void UnknownSchoolYearDoesNotRequireConsent()
    {
        Assert.False(Year(null).RequiresGuardianConsent);
    }

    [Fact]
    public void SoftDeletedStudentIsNotActive()
    {
        var student = Year(10);
        student.SoftDelete(DateTimeOffset.UtcNow);

        Assert.False(student.IsActive);
    }

    [Fact]
    public void TwoStudentsWithDifferentIdsAreNotEqual()
    {
        Assert.NotEqual(Year(10), Year(10));
    }
}
