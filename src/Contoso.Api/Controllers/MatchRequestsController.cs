using Contoso.Application.Matching;
using Contoso.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("api/match-requests")]
public sealed class MatchRequestsController(MatchRequestService requests) : ControllerBase
{
    public sealed record CreateMatchRequest(Guid StudentId, Guid MentorId);

    public sealed record AnswerMatchRequest(bool Accept, string? Reason);

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateMatchRequest body, CancellationToken ct)
    {
        try
        {
            var created = await requests.RequestAsync(body.StudentId, body.MentorId, User.Identity?.Name ?? "anonymous", ct);
            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Mentor not found", Detail = ex.Message });
        }
        catch (DomainException ex)
        {
            return Conflict(new ProblemDetails { Title = "Request rejected", Detail = ex.Message });
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Answer(Guid id, [FromBody] AnswerMatchRequest body, CancellationToken ct)
    {
        try
        {
            var answered = await requests.AnswerAsync(id, body.Accept, User.Identity?.Name ?? "anonymous", body.Reason, ct);
            return Ok(answered);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Title = "Request not found", Detail = ex.Message });
        }
        catch (DomainException ex)
        {
            return Conflict(new ProblemDetails { Title = "Already answered", Detail = ex.Message });
        }
    }
}
