using System.Security.Claims;
using CvAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CvAPI2.DTO;
using Microsoft.EntityFrameworkCore;
using CvAPI2.Models.Tag;


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

    [Authorize(Roles = "User,Admin")]
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
                DateOfBirth = cv.DateOfBirth,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",

                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    Company = w.CompanyName,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
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

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
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


    [Authorize(Roles = "User,Admin")]
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
                DateOfBirth = cv.DateOfBirth,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",

                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto 
                {
                    Company = w.CompanyName, 
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Position = w.Position, 
                    From = w.StartDate, 
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>() 
                }).ToList(),

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    Organization = a.Organization,
                    AwardDescription = a.AwardDescription,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
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

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
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

            var newCv = new Cv
            {
                Personalia = dtoCreate.Personalia,
                DateOfBirth = dtoCreate.DateOfBirth,
                UserId = userId.ToLower(),

                Educations = dtoCreate.Educations?.Select(e => new Education
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartDate = e.StartYear,
                    EndDate = e.EndYear
                }).ToList() ?? new List<Education>(),

                Awards = dtoCreate.Awards?.Select(a => new Award
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList() ?? new List<Award>(),

                Certifications = dtoCreate.Certifications?.Select(c => new Certification
                {
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList() ?? new List<Certification>(),

                Courses = dtoCreate.Courses?.Select(c => new Course
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
                    Provider = c.Provider,
                    Date = c.CompletionDate
                }).ToList() ?? new List<Course>(),

                CompetenceOverviews = dtoCreate.CompetenceOverviews?.Select(c => new CompetenceOverview
                {
                    skill_name = c.Area,
                    skill_level = c.Level
                }).ToList() ?? new List<CompetenceOverview>(),

                Languages = dtoCreate.Languages?.Select(l => new Language
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<Language>(),

                RoleOverviews = dtoCreate.RoleOverviews?.Select(r => new RoleOverview
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                }).ToList() ?? new List<RoleOverview>(),
                
                WorkExperiences = new List<WorkExperience>(),
                ProjectExperiences = new List<ProjectExperience>()
            };
            if (dtoCreate.WorkExperiences != null)
            {
                foreach (var w in dtoCreate.WorkExperiences)
                {
                    var work = new WorkExperience
                    {
                        CompanyName = w.Company,
                        Position = w.Position,
                        WorkExperienceDescription = w.WorkExperienceDescription,
                        StartDate = w.From,
                        EndDate = w.To,
                        Tags = new List<WorkExperienceTag>()
                    };
                    foreach (var tagValue in w.Tags ?? new List<string>())
                    {
                        var existingTag = await _cvService.GetOrCreateTagAsync(tagValue);
                        work.Tags.Add(new WorkExperienceTag
                        {

                            Tag = existingTag,
                            TagId = existingTag.Id
                        });
                    }
                    newCv.WorkExperiences.Add(work);
                }
            }
            if (dtoCreate.ProjectExperiences != null)
            {
                foreach (var p in dtoCreate.ProjectExperiences)
                {
                    var work = new ProjectExperience
                    {
                        ProjectName = p.ProjectName,
                        CompanyName = p.CompanyName,
                        ProjectExperienceDescription = p.ProjectExperienceDescription,
                        StartDate = p.StartDate,
                        Role = p.Role,
                        EndDate = p.EndDate,
                        Tags = new List<ProjectExperienceTag>()
                    };
                    foreach (var tagValue in p.Tags ?? new List<string>())
                    {
                        var existingTag = await _cvService.GetOrCreateTagAsync(tagValue);

                        work.Tags.Add(new ProjectExperienceTag
                        {  
                            Tag = existingTag,
                            TagId = existingTag.Id
                        });
                    }
                    newCv.ProjectExperiences.Add(work);
                }
            }

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
    public async Task<IActionResult> UpdateCv(string id, [FromBody] UpdateCvDto dtoCreateCv)
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

            existingCv.Personalia = dtoCreateCv.Personalia;

            existingCv.WorkExperiences = new List<WorkExperience>();

            if (dtoCreateCv.WorkExperiences != null)
            {
                foreach (var w in dtoCreateCv.WorkExperiences)
                {
                    var work = new WorkExperience
                    {
                        CompanyName = w.Company,
                        Position = w.Position,
                        WorkExperienceDescription = w.WorkExperienceDescription,
                        StartDate = w.From,
                        EndDate = w.To,
                        CvId = existingCv.Id,
                        Tags = new List<WorkExperienceTag>()
                    };

                    foreach (var tagValue in w.Tags ?? new List<string>())
                    {
                        var tag = await _cvService.GetOrCreateTagAsync(tagValue);
                        work.Tags.Add(new WorkExperienceTag
                        {
                            TagId = tag.Id,
                            Tag = tag
                        });
                    }

                    existingCv.WorkExperiences.Add(work);
                }
            }


            existingCv.Educations = dtoCreateCv.Educations?.Select(e => new Education
            {
                School = e.School,
                Degree = e.Degree,
                EducationDescription = e.EducationDescription,
                StartDate = e.StartYear,
                EndDate = e.EndYear,
                CvId = existingCv.Id
            }).ToList() ?? new List<Education>();

            existingCv.Awards = dtoCreateCv.Awards?.Select(a => new Award
            {
                Name = a.Name,
                AwardDescription = a.AwardDescription,
                Organization = a.Organization,
                Year = a.Year,
                CvId = existingCv.Id
            }).ToList() ?? new List<Award>();

            existingCv.Certifications = dtoCreateCv.Certifications?.Select(c => new Certification
            {
                Name = c.Name,
                IssuedBy = c.IssuedBy,
                CertificationDescription = c.CertificationDescription,
                Date = c.Date,
                CvId = existingCv.Id
            }).ToList() ?? new List<Certification>();

            existingCv.Courses = dtoCreateCv.Courses?.Select(c => new Course
            {
                Name = c.Name,
                Provider = c.Provider,
                CourseDescription = c.CourseDescription,
                Date = c.CompletionDate,
                CvId = existingCv.Id
            }).ToList() ?? new List<Course>();

            existingCv.CompetenceOverviews = dtoCreateCv.CompetenceOverviews?.Select(c => new CompetenceOverview
            {
                skill_name = c.Area,
                skill_level = c.Level,
                CvId = existingCv.Id
            }).ToList() ?? new List<CompetenceOverview>();

            existingCv.Languages = dtoCreateCv.Languages?.Select(l => new Language
            {
                Name = l.Name,
                Proficiency = l.Proficiency,
                CvId = existingCv.Id
            }).ToList() ?? new List<Language>();

            existingCv.ProjectExperiences = new List<ProjectExperience>();

            if (dtoCreateCv.ProjectExperiences != null)
            {
                foreach (var p in dtoCreateCv.ProjectExperiences)
                {
                    var project = new ProjectExperience
                    {
                        ProjectName = p.ProjectName,
                        ProjectExperienceDescription = p.ProjectExperienceDescription,
                        CompanyName = p.CompanyName,
                        Role = p.Role,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        CvId = existingCv.Id,
                        Tags = new List<ProjectExperienceTag>()
                    };

                    foreach (var tagValue in p.Tags ?? new List<string>())
                    {
                        var tag = await _cvService.GetOrCreateTagAsync(tagValue);
                        project.Tags.Add(new ProjectExperienceTag
                        {
                            TagId = tag.Id,
                            Tag = tag
                        });
                    }

                    existingCv.ProjectExperiences.Add(project);
                }
            }


            existingCv.RoleOverviews = dtoCreateCv.RoleOverviews?.Select(r => new RoleOverview
            {
                Role = r.Role,
                RoleDescription = r.RoleDescription,
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
            if (existingCv != null)
            {
                _logger.LogInformation("Bruker {UserId} har allerede en CV med ID {CvId}", userId, existingCv.Id);
                return BadRequest("Brukeren har allerede en CV!");
            }

            var newCv = new Cv
            {
                Personalia = dto.Personalia,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                UserId = userId,
                Educations = dto.Educations?.Select(e => new Education
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartDate = e.StartYear,
                    EndDate = e.EndYear
                }).ToList() ?? new List<Education>(),

                Awards = dto.Awards?.Select(a => new Award
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList() ?? new List<Award>(),

                Certifications = dto.Certifications?.Select(c => new Certification
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    CertificationDescription = c.CertificationDescription,
                    Date = c.Date
                }).ToList() ?? new List<Certification>(),

                Courses = dto.Courses?.Select(c => new Course
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    CourseDescription = c.CourseDescription,
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

                RoleOverviews = dto.RoleOverviews?.Select(r => new RoleOverview
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                }).ToList() ?? new List<RoleOverview>(),

                WorkExperiences = new List<WorkExperience>(),
                ProjectExperiences = new List<ProjectExperience>()
            };

            if (dto.WorkExperiences != null)
            {
                foreach (var w in dto.WorkExperiences)
                {
                    var work = new WorkExperience
                    {
                        CompanyName = w.Company,
                        WorkExperienceDescription = w.WorkExperienceDescription,
                        Position = w.Position,
                        StartDate = w.From,
                        EndDate = w.To,
                        Tags = new List<WorkExperienceTag>()
                    };

                    foreach (var tagValue in w.Tags ?? new List<string>())
                    {
                        var existingTag = await _cvService.GetOrCreateTagAsync(tagValue);
                        work.Tags.Add(new WorkExperienceTag
                        {
                            Tag = existingTag,
                            TagId = existingTag.Id // 🔥 viktig!
                        });
                    }

                    newCv.WorkExperiences.Add(work);
                }
            }

            if (dto.ProjectExperiences != null)
            {
                foreach (var p in dto.ProjectExperiences)
                {
                    var project = new ProjectExperience
                    {
                        ProjectName = p.ProjectName,
                        CompanyName = p.CompanyName,
                        ProjectExperienceDescription = p.ProjectExperienceDescription,
                        Role = p.Role,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Tags = new List<ProjectExperienceTag>()
                    };

                    foreach (var tagValue in p.Tags ?? new List<string>())
                    {
                        var existingTag = await _cvService.GetOrCreateTagAsync(tagValue);
                        project.Tags.Add(new ProjectExperienceTag
                        {
                            Tag = existingTag,
                            TagId = existingTag.Id // 🔥 viktig!
                        });
                    }

                    newCv.ProjectExperiences.Add(project);
                }
            }

            await _cvService.AddCv(newCv);
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

            var dtoResults = results.Select(cv => new CvDto
            {
                Id = cv.Id,
                Personalia = cv.Personalia,
                DateOfBirth = cv.DateOfBirth,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",


                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    Company = w.CompanyName,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>(),
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate
                }).ToList(),

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
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

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>(),
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                }).ToList()

            }).ToList();
                
            _logger.LogInformation("Fant {Count} Cv-er som er matchet søket", dtoResults.Count);
            return Ok(dtoResults);
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

}