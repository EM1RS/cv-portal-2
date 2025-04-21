using System.Security.Claims;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Route("api/me")]
[ApiController]

public class MeController : ControllerBase
{
    private readonly ICvService _cvService;
    private readonly ILogger<MeController> _logger;
    private readonly UserManager<User> _userManager;

    public MeController(ICvService cvService, ILogger<MeController> logger, UserManager<User> userManager)
    {
        _cvService = cvService;
        _logger = logger;
        _userManager = userManager;
    }
    

    [Authorize(Roles = "User,Admin")]
    [HttpGet("cv")]
    public async Task<IActionResult> GetCvForUser()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Ingen bruker-ID funnet i token.");
                return Unauthorized("Bruker ikke identifisert.");
            }

            var dto = await _cvService.GetCvProfileForUser(userId);
            if (dto == null)
            {
                _logger.LogInformation("Ingen CV funnet for bruker {UserId}.", userId);
                return NotFound("Fant ingen CV for denne brukeren.");
            }            
            return Ok(dto);  
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av egen CV.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("user")]

    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Ingen bruker-ID funnet i token.");
                return Unauthorized("Bruker ikke identifisert.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogInformation("Ingen bruker funnet med ID {UserId}.", userId);
                return NotFound("Fant ingen bruker med denne ID-en.");
            }

            var dto = new MeProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av brukerinfo.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

    [Authorize (Roles = "User,Admin")]
    [HttpPut("user")]
    public async Task<IActionResult> UpdateMyUser([FromBody] MeEditDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Bruker ikke funnet.");

        user.FullName = dto.FullName;
        user.Email = dto.Email;

        // Hvis bruker prøver å endre passord
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest("Nytt passord og bekreftelse stemmer ikke overens.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
        }

        await _userManager.UpdateAsync(user);
        return NoContent();
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut("cv")]
    public async Task<IActionResult> UpdateMyCv([FromBody] UpdateCvDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Ingen bruker-ID funnet i token.");
                return Unauthorized("Bruker ikke identifisert.");
            }

            var existingCv = await _cvService.GetCvForUser(userId);
            if (existingCv == null)
            {
                _logger.LogInformation("Bruker {UserId} har ingen CV å oppdatere.", userId);
                return NotFound("Du har ikke en CV å oppdatere.");
            }

            await _cvService.UpdateMyCv(existingCv, dto);

            _logger.LogInformation("Bruker {UserId} oppdaterte sin CV (ID: {CvId})", userId, existingCv.Id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved oppdatering av egen CV.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


}
