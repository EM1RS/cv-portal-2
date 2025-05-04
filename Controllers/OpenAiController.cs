using System.Security.Claims;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/AI")]
[ApiController]

public class OpenAIController : ControllerBase
{
    private readonly IPromptService _promptService;
    private readonly ILogger<OpenAIController> _logger;
    private readonly UserManager<User> _userManager;

    public OpenAIController(IPromptService promptService, ILogger<OpenAIController> logger, UserManager<User> userManager)
    {
        _promptService = promptService;
        _logger = logger;
        _userManager = userManager;
    }

    
    [HttpPost("summary/{cvId}")]
    public async Task<IActionResult> GenerateAndSaveSummary(string cvId)
    {
        var summary = await _promptService.GenerateAndOptionallySaveSummaryAsync(cvId, save: false);
        
        if (summary == null)
        {
            return NotFound(new { message = "Ingen CV funnet." });
        }

        return Ok(new { message = "Sammendrag generert.", summary });
    }
    
    [HttpPost("summary/save/{cvId}")]
    public async Task<IActionResult> SaveSummary(string cvId, [FromBody] string summaryText)
    {
        await _promptService.SaveSummaryToDatabaseAsync(cvId, summaryText);
        return Ok(new { message = "Sammendrag lagret." });
    }


    [HttpGet("summaries")]
    public async Task<IActionResult> GetAllSummaries()
    {
        var summaries = await _promptService.GetAllSummariesAsync();

        if (summaries == null || !summaries.Any())
        {
            return NotFound(new { message = "Ingen sammendrag funnet." });
        }

        return Ok(summaries);
    }

    [HttpDelete("summary/{summaryId}")]
    public async Task<IActionResult> DeleteSummary(string summaryId)
    {
        await _promptService.DeleteSummaryAsync(summaryId);
        return Ok(new { message = "Sammendrag slettet." });
    }


    [HttpPost("evaluate-candidate/{cvId}")]
    public async Task<IActionResult> EvaluateCandidate(string cvId, [FromBody] string requirements)
    {
        var evaluation = await _promptService.EvaluateCandidateAsync(cvId, requirements);
        return Ok(new { result = evaluation });
    }



}