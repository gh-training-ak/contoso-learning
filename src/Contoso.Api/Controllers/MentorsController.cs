using Contoso.Application.Abstractions;
using Contoso.Application.Common;
using Contoso.Application.Mentors;
using Contoso.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("api/mentors")]
public sealed class MentorsController(MentorSearchService search, IMentorRepository mentors) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MentorSearchResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] Subject? subject,
        [FromQuery] MeetingType? meetingType,
        [FromQuery] decimal? maxHourlyRate,
        [FromQuery] decimal? minimumRating,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var criteria = new MentorSearchCriteria
        {
            Subject = subject,
            MeetingType = meetingType,
            MaxHourlyRate = maxHourlyRate,
            MinimumRating = minimumRating,
            Page = Math.Max(page, 1),
            PageSize = Math.Clamp(pageSize, 1, MentorSearchService.MaxPageSize)
        };

        return Ok(await search.SearchAsync(criteria, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MentorSearchResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var mentor = await mentors.GetByIdAsync(id, ct);

        if (mentor is null || !mentor.IsActive)
        {
            return NotFound();
        }

        return Ok(new MentorSearchResult(
            mentor.Id,
            mentor.DisplayName,
            mentor.HourlyRate.Amount,
            mentor.HourlyRate.Currency,
            mentor.AverageRating,
            mentor.Reviews.Count,
            null));
    }
}
