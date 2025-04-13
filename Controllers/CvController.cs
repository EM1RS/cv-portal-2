using System.Security.Claims;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CvAPI2.DTO;
using Microsoft.EntityFrameworkCore;


[Route("api/cvs")]
[ApiController]
public class CvController : ControllerBase
{
    private readonly ICvService _cvService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CvController> _logger;

    public CvController(ICvService cvService, UserManager<User> userManager, ILogger<CvController> logger)
    {
        _cvService = cvService;
        _userManager = userManager;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllCvs()
    {
        try
        {
            var cvs = await _cvService.GetAllCvs();

            var cvDtos = cvs.Select(cv => new CvDto
            {
                Id = cv.Id,
                Personalia = cv.Personalia,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",

                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    Company = w.Company,
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate
                }).ToList(),

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    Description = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    CompletionDate = c.Date
                }).ToList(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(c => new CompetenceOverviewDto
                {
                    Area = c.skill_name,
                    Level = c.skill_level
                }).ToList(),

                Languages = cv.Languages?.Select(l => new LanguageDto
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList(),

                Positions = cv.Positions?.Select(p => new PositionDto
                {
                    Title = p.Name,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                RolesOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    Description = r.Description
                }).ToList()

            }).ToList();

            return Ok(cvDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av alle CV-er.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCvById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(ApiResponse<string>.Fail("Cv-ID mangler"));

        try
        {
            var cv = await _cvService.GetCvById(id);
            if (cv == null)
                return NotFound(ApiResponse<string>.Fail("CV ikke funnet"));

            var cvDto = new CvDto {
                Id = cv.Id,
                Personalia = cv.Personalia,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",

                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto {Company = w.Company, Position = w.Position, From = w.StartDate, To = w.EndDate }).ToList(),

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    Description = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    CompletionDate = c.Date
                }).ToList(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(c => new CompetenceOverviewDto
                {
                    Area = c.skill_name,
                    Level = c.skill_level
                }).ToList(),

                Languages = cv.Languages?.Select(l => new LanguageDto
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList(),

                Positions = cv.Positions?.Select(p => new PositionDto
                {
                    Title = p.Name,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                RolesOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    Description = r.Description
                }).ToList()
            };

            return Ok(ApiResponse<CvDto>.Ok(cvDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av CV med ID {Id}", id);
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }



    [Authorize(Roles = "User,Admin")]
    [HttpGet("mine")]
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

            var cv = await _cvService.GetCvForUser(userId);
            if (cv == null)
            {
                _logger.LogInformation("Ingen CV funnet for bruker {UserId}.", userId);
                return NotFound("Fant ingen CV for denne brukeren.");
            }

            return Ok(cv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av egen CV.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCv([FromBody] CreateCvDto dtoUpdate)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Ingen bruker-ID funnet i token.");
                return Unauthorized("Ingen bruker identifisert.");
            }

            var newCv = new Cv
            {
                Personalia = dtoUpdate.Personalia,
                UserId = userId.ToLower(),

                Educations = dtoUpdate.Educations?.Select(e => new Education
                {
                    School = e.School,
                    Degree = e.Degree,
                    StartDate = e.StartYear,
                    EndDate = e.EndYear
                }).ToList() ?? new List<Education>(),

                WorkExperiences = dtoUpdate.WorkExperiences?.Select(w => new WorkExperience
                {
                    Company = w.Company,
                    Position = w.Position,
                    StartDate = w.From,
                    EndDate = w.To
                }).ToList() ?? new List<WorkExperience>(),

                Awards = dtoUpdate.Awards?.Select(a => new Award
                {
                    Name = a.Name,
                    Organization = a.Description,
                    Year = a.Year
                }).ToList() ?? new List<Award>(),

                Certifications = dtoUpdate.Certifications?.Select(c => new Certification
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList() ?? new List<Certification>(),

                Courses = dtoUpdate.Courses?.Select(c => new Course
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    Date = c.CompletionDate
                }).ToList() ?? new List<Course>(),

                CompetenceOverviews = dtoUpdate.CompetenceOverviews?.Select(c => new CompetenceOverview
                {
                    skill_name = c.Area,
                    skill_level = c.Level
                }).ToList() ?? new List<CompetenceOverview>(),

                Languages = dtoUpdate.Languages?.Select(l => new Language
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<Language>(),

                Positions = dtoUpdate.Positions?.Select(p => new Position
                {
                    Name = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList() ?? new List<Position>(),

                ProjectExperiences = dtoUpdate.ProjectExperiences?.Select(p => new ProjectExperience
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList() ?? new List<ProjectExperience>(),

                RoleOverviews = dtoUpdate.RolesOverviews?.Select(r => new RoleOverview
                {
                    Role = r.Role,
                    Description = r.Description
                }).ToList() ?? new List<RoleOverview>()
            };

            await _cvService.AddCv(newCv);
            _logger.LogInformation("Ny CV opprettet for bruker {UserId} med CV-ID {CvId}", userId, newCv.Id);

            return CreatedAtAction(nameof(GetCvForUser), new { }, newCv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved opprettelse av CV.");
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [Authorize(Roles = "User,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCv(string id, [FromBody] UpdateCvDto dtoUpdateCv)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCv = await _cvService.GetCvById(id);

            if (existingCv == null)
            {
                _logger.LogWarning("Oppdatering feilet: CV med ID {Id} ble ikke funnet.", id);
                return NotFound("Cv ikke funnet!");
            }

            var isAdmin = User.IsInRole("Admin");
            if (existingCv.UserId != userId && !isAdmin)
            {
                _logger.LogWarning("Bruker {UserId} har ikke tilgang til å oppdatere CV med ID {CvId}", userId, id);
                return Forbid("Du har ikke tilgang til å oppdatere denne CV-en!");
            }

            existingCv.Personalia = dtoUpdateCv.Personalia;

            existingCv.WorkExperiences = dtoUpdateCv.WorkExperiences?.Select(w => new WorkExperience
            {
                Company = w.Company,
                Position = w.Position,
                StartDate = w.From,
                EndDate = w.To,
                CvId = existingCv.Id
            }).ToList() ?? new List<WorkExperience>();

            existingCv.Educations = dtoUpdateCv.Educations?.Select(e => new Education
            {
                School = e.School,
                Degree = e.Degree,
                StartDate = e.StartYear,
                EndDate = e.EndYear,
                CvId = existingCv.Id
            }).ToList() ?? new List<Education>();

            existingCv.Awards = dtoUpdateCv.Awards?.Select(a => new Award
            {
                Name = a.Name,
                Organization = a.Description,
                Year = a.Year,
                CvId = existingCv.Id
            }).ToList() ?? new List<Award>();

            existingCv.Certifications = dtoUpdateCv.Certifications?.Select(c => new Certification
            {
                Name = c.Name,
                IssuedBy = c.IssuedBy,
                Date = c.Date,
                CvId = existingCv.Id
            }).ToList() ?? new List<Certification>();

            existingCv.Courses = dtoUpdateCv.Courses?.Select(c => new Course
            {
                Name = c.Name,
                Provider = c.Provider,
                Date = c.CompletionDate,
                CvId = existingCv.Id
            }).ToList() ?? new List<Course>();

            existingCv.CompetenceOverviews = dtoUpdateCv.CompetenceOverviews?.Select(c => new CompetenceOverview
            {
                skill_name = c.Area,
                skill_level = c.Level,
                CvId = existingCv.Id
            }).ToList() ?? new List<CompetenceOverview>();

            existingCv.Languages = dtoUpdateCv.Languages?.Select(l => new Language
            {
                Name = l.Name,
                Proficiency = l.Proficiency,
                CvId = existingCv.Id
            }).ToList() ?? new List<Language>();

            existingCv.Positions = dtoUpdateCv.Positions?.Select(p => new Position
            {
                Name = p.Title,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                CvId = existingCv.Id
            }).ToList() ?? new List<Position>();

            existingCv.ProjectExperiences = dtoUpdateCv.ProjectExperiences?.Select(p => new ProjectExperience
            {
                ProjectName = p.ProjectName,
                Description = p.Description,
                Role = p.Role,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                CvId = existingCv.Id
            }).ToList() ?? new List<ProjectExperience>();

            existingCv.RoleOverviews = dtoUpdateCv.RolesOverviews?.Select(r => new RoleOverview
            {
                Role = r.Role,
                Description = r.Description,
                CvId = existingCv.Id
            }).ToList() ?? new List<RoleOverview>();

            await _cvService.UpdateCv(existingCv);
            _logger.LogInformation("CV med ID {CvId} oppdatert av bruker {UserId}", id, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved oppdatering av CV med ID {CvId}", id);
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCv(string id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCv = await _cvService.GetCvById(id);

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
            _logger.LogInformation("🔍 CV funnet for bruker {UserId}: {HasCv}", userId, existingCv != null);

            if (existingCv != null)
            {
                _logger.LogInformation("Bruker {UserId} har allerede en CV med ID {CvId}", userId, existingCv.Id);
                return BadRequest("Brukeren har allerede en CV!");
            }

            var newCv = new Cv
            {
                Personalia = dto.Personalia,
                UserId = userId,
                Educations = dto.Educations?.Select(e => new Education
                {
                    School = e.School,
                    Degree = e.Degree,
                    StartDate = e.StartYear,
                    EndDate = e.EndYear
                }).ToList() ?? new List<Education>(),

                WorkExperiences = dto.WorkExperiences?.Select(w => new WorkExperience
                {
                    Company = w.Company,
                    Position = w.Position,
                    StartDate = w.From,
                    EndDate = w.To
                }).ToList() ?? new List<WorkExperience>(),

                Awards = dto.Awards?.Select(a => new Award
                {
                    Name = a.Name,
                    Organization = a.Description,
                    Year = a.Year
                }).ToList() ?? new List<Award>(),

                Certifications = dto.Certifications?.Select(c => new Certification
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList() ?? new List<Certification>(),

                Courses = dto.Courses?.Select(c => new Course
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    Date = c.CompletionDate
                }).ToList() ?? new List<Course>(),

                CompetenceOverviews = dto.CompetenceOverviews?.Select(c => new CompetenceOverview
                {
                    skill_name = c.Area,
                    skill_level = c.Level
                }).ToList() ?? new List<CompetenceOverview>(),

                Languages = dto.Languages?.Select(l => new Language
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<Language>(),

                Positions = dto.Positions?.Select(p => new Position
                {
                    Name = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList() ?? new List<Position>(),

                ProjectExperiences = dto.ProjectExperiences?.Select(p => new ProjectExperience
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList() ?? new List<ProjectExperience>(),

                RoleOverviews = dto.RolesOverviews?.Select(r => new RoleOverview
                {
                    Role = r.Role,
                    Description = r.Description
                }).ToList() ?? new List<RoleOverview>()
            };

            await _cvService.AddCv(newCv);
            _logger.LogInformation("Ny CV opprettet for bruker {UserId} med Cv-ID {CvId}", userId, newCv.Id);
            return CreatedAtAction(nameof(GetCvById), new { id = newCv.Id }, newCv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved opprettelse av CV for bruker {UserId}", userId);
            return StatusCode(500, new ApiError { Message = "Uventet feil oppstod.", Details = ex.Message });
        }
    }

}