using System.Security.Claims;
using CvApi2.Service;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/cvs")]
[ApiController]

public class CvController : ControllerBase
{
    private readonly ICvService _cvService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CvController> _logger;

    private readonly S3Service _s3Service;

    public CvController(ICvService cvService, UserManager<User> userManager, ILogger<CvController> logger, S3Service s3Service)
    {
        _cvService = cvService;
        _s3Service = s3Service;
        _userManager = userManager;
        _logger = logger;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllCvs()
    {
        try
        {
            var cvDtos = await _cvService.GetAllCvDtos();
            return Ok(cvDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av alle CV-er.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCvById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Cv-ID mangler");

        try
        {
            var cvDto = await _cvService.GetCvDtoById(id);
            if (cvDto == null)
                return NotFound("CV ikke funnet");

            return Ok(cvDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av CV med ID {Id}", id);
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "User, Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCv([FromBody] CreateCvDto dtoCreate)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Ingen bruker-ID funnet i token.");
                return Unauthorized("Ingen bruker identifisert.");
            }

            var newCv = await _cvService.CreateCvFromDto(userId, dtoCreate);
            _logger.LogInformation("Ny CV opprettet for bruker {UserId} med CV-ID {CvId}", userId, newCv.Id);

            return CreatedAtAction(nameof(MeController.GetCvForUser), new { }, newCv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved opprettelse av CV.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

   


    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCv(string id, [FromBody] UpdateCvDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var existingCv = await _cvService.GetCvById(id);

            if (existingCv == null)
            {
                _logger.LogWarning("Oppdatering feilet: CV med ID {Id} ble ikke funnet.", id);
                return NotFound(new ApiError { Message = "CV ikke funnet!" });
            }

            if (existingCv.UserId != userId && !isAdmin)
            {
                _logger.LogWarning("Bruker {UserId} har ikke tilgang til å oppdatere CV med ID {CvId}", userId, id);
                return StatusCode(StatusCodes.Status403Forbidden, new ApiError
                {
                    Message = "Du har ikke tilgang til å oppdatere denne CV-en."
                });
            }

            await _cvService.UpdateCvFromDto(existingCv, dto);

            _logger.LogInformation("CV med ID {CvId} oppdatert av bruker {UserId}", id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved oppdatering av CV med ID {CvId}", id);
            return StatusCode(500, new ApiError
            {
                Message = "Uventet feil oppstod.",
                Details = ex.Message
            });
        }
    }




    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCv(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCv = await _cvService.GetCvDtoById(id);

            if (existingCv == null)
            {
                _logger.LogWarning("Sletting feilet: CV med ID {CvId} ble ikke funnet.", id);
                return NotFound("CV ikke funnet!");
            }

            var isAdmin = User.IsInRole("Admin");
            if (existingCv.UserId != userId && !isAdmin)
            {
                _logger.LogWarning("Bruker {UserId} har ikke tilgang til å slette CV med ID {CvId}", userId, id);
                return Forbid("Du har ikke tilgang til å slette denne CV-en.");
            }

            await _cvService.DeleteCv(id);
            _logger.LogInformation("CV med ID {CvId} slettet av bruker {UserId}", id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved sletting av CV med ID {CvId}", id);
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("for-user/{userId}")]
    public async Task<IActionResult> CreateCvForUser(string userId, [FromBody] CreateCvDto dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Opprettelse feilet: Bruker med ID {UserId} ble ikke funnet.", userId);
                return NotFound("Bruker ikke funnet.");
            }

            var existingCv = await _cvService.GetCvForUser(userId);
            if (existingCv != null)
            {
                _logger.LogInformation("Bruker {UserId} har allerede en CV med ID {CvId}", userId, existingCv.Id);
                return BadRequest("Brukeren har allerede en CV!");
            }

            var newCv = await _cvService.CreateCvFromDto(userId, dto);
            _logger.LogInformation("Ny CV opprettet for bruker {UserId} med Cv-ID {CvId}", userId, newCv.Id);

            return CreatedAtAction(nameof(GetCvById), new { id = newCv.Id }, newCv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved opprettelse av CV for bruker {UserId}", userId);
            return StatusCode(500, new ApiError
            {
                Message = "Uventet feil oppstod.",
                Details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchCvs([FromQuery] string keywords)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return BadRequest("Søkeord må oppgis!");

            var terms = keywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim().ToLower())
                .ToList();

            _logger.LogInformation("Søker etter Cv-er med nøkkelord: {Keywords}", string.Join(", ", terms));

            var results = await _cvService.SearchCvsByKeywords(terms);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("Ingen resultater funnet for nøkkelord: {Keywords}", string.Join(", ", terms));
                return NotFound(new ApiError
                {
                    Message = "Ingen CV-er matchet søket.",
                    Details = "Prøv å søke med andre eller flere nøkkelord."
                });
            }

            _logger.LogInformation("Fant {Count} Cv-er som matchet søket", results.Count);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved søk etter Cv-er med nøkkelord: {Keywords}", keywords);
            return StatusCode(500, new ApiError
            {
                Message = "Uventet feil oppstod under søk!",
                Details = ex.Message
            });
        }
    }
    [HttpPost("{cvId}/upload-image")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UploadProfileImage(string cvId, IFormFile file)
    {
        var cv = await _cvService.GetCvById(cvId);
        if (cv == null)
            return NotFound("CV ikke funnet.");

        var fileName = $"{cvId}_{Path.GetFileName(file.FileName)}";
        var imageUrl = await _s3Service.UploadAsync(file, fileName);

        var updateDto = new UpdateCvDto
        {
            ProfileImageUrl = imageUrl
        };
        await _cvService.UpdateCvFromDto(cv, updateDto); 

        return Ok(new { imageUrl });
    }
    
}