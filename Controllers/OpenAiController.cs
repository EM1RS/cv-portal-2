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
        try
        {
            var summary = await _promptService.GenerateAndOptionallySaveSummaryAsync(cvId, save: false);

            if (summary == null)
                return NotFound(ApiResponse<string>.Fail("CV ikke funnet."));

            return Ok(ApiResponse<string>.Ok(summary, "Sammendrag generert."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved generering av sammendrag.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

    [HttpPost("summary/save/{cvId}")]
    public async Task<IActionResult> SaveSummary(string cvId, [FromBody] string summaryText)
    {
        try
        {
            var success = await _promptService.SaveSummaryToDatabaseAsync(cvId, summaryText);
            if (!success)
                return NotFound(ApiResponse<string>.Fail("CV ikke funnet, kunne ikke lagre sammendrag."));

            return Ok(ApiResponse<string>.Ok(summaryText, "Sammendrag lagret."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved lagring av sammendrag.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

    [HttpGet("summary/{cvId}")]
    public async Task<IActionResult> GetSummaryById(string cvId)
    {
        try
        {
            var summary = await _promptService.GetSummaryByIdAsync(cvId);

            if (summary == null)
                return NotFound(ApiResponse<string>.Fail($"Ingen sammendrag funnet for ID: {cvId}."));

            return Ok(ApiResponse<CvSummary>.Ok(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av sammendrag.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

    [HttpDelete("summary/{summaryId}")]
    public async Task<IActionResult> DeleteSummary(string summaryId)
    {
        try
        {
            await _promptService.DeleteSummaryAsync(summaryId);
            return Ok(ApiResponse<string>.Ok(summaryId, "Sammendrag slettet."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved sletting av sammendrag.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }
    [Authorize (Roles = "Admin")]
    [HttpPost("MatrixRequirements/{cvId}")]
    public async Task<IActionResult> MatrixRequirements(string cvId, [FromBody] string requirements)
    {
        try
        {
            var evaluation = await _promptService.MatrixRequirementAsync(cvId, requirements);

            if (evaluation == null)
                return NotFound(ApiResponse<string>.Fail("CV ikke funnet for evaluering."));

            return Ok(ApiResponse<string>.Ok(evaluation, "Evaluering gjennomført."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved evaluering av kandidat.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }
}
