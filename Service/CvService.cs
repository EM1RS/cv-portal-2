using CvAPI2.Models;
using CvAPI2.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


public class CvService : ICvService

{
    private readonly ICvRepository _CvRepository;
    private readonly ILogger<CvService> _logger;

    public CvService(ICvRepository CvRepository, ILogger<CvService> logger)
    {
        _logger = logger;
        _CvRepository = CvRepository;
    }

    public Task<IEnumerable<Cv>> GetAllCvs() => _CvRepository.GetAllCvs();

    public Task<Cv?> GetCvById(string id) => _CvRepository.GetCvById(id);

    public async Task<Cv?> GetCvForUser(string userId)
    {
        return await _CvRepository.GetCvByUserId(userId);
    }
    
    public async Task DeleteCv(string id) => await _CvRepository.DeleteCv(id);

    public async Task<Tag> GetOrCreateTagAsync(string value)
    {
        return await _CvRepository.GetOrCreateTagAsync(value);
    }

  public async Task UpdateMyCv(Cv existingCv, UpdateCvDto dto)
    {
        try
        {
            _logger.LogInformation("Starter oppdatering av CV med ID: {CvId}", existingCv.Id);
            await UpdateCvFromDto(existingCv, dto);
            _logger.LogInformation("Oppdatering av CV med ID {CvId} fullført", existingCv.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil i UpdateMyCv for CV-ID {CvId}", existingCv.Id);
            throw; 
        }
    }



    public async Task<CvProfileDto?> GetCvProfileForUser(string userId)
    {
        try
        {
            _logger.LogInformation("Henter CV-profil for bruker {UserId}", userId);

            var cv = await GetCvForUser(userId);
            if (cv == null)
            {
                _logger.LogWarning("Ingen CV funnet for bruker {UserId}", userId);
                return null;
            }

            _logger.LogInformation("Fant CV med ID {CvId} for bruker {UserId}", cv.Id, userId);

            return new CvProfileDto
            {
                Id = cv.Id,
                Personalia = cv.Personalia,
                DateOfBirth = cv.DateOfBirth,
                PhoneNumber = cv.PhoneNumber,

                Educations = cv.Educations?.Select(e => new EducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList() ?? new List<EducationDto>(),

                Awards = cv.Awards?.Select(a => new AwardDto
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList() ?? new List<AwardDto>(),

                Certifications = cv.Certifications?.Select(c => new CertificationDto
                {
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    CertificationDescription = c.CertificationDescription,
                    Date = c.Date
                }).ToList() ?? new List<CertificationDto>(),

                Courses = cv.Courses?.Select(c => new CourseDto
                {
                    Name = c.Name,
                    Provider = c.Provider,
                    CourseDescription = c.CourseDescription,
                    CompletionDate = c.Date
                }).ToList() ?? new List<CourseDto>(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(co => new CompetenceOverviewDto
                {
                    Area = co.skill_name,
                    Level = co.skill_level
                }).ToList() ?? new List<CompetenceOverviewDto>(),

                Languages = cv.Languages?.Select(l => new LanguageDto
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<LanguageDto>(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new RoleOverviewDto
                {
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                }).ToList() ?? new List<RoleOverviewDto>(),

                WorkExperiences = cv.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    Company = w.CompanyName,
                    Position = w.Position,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    From = w.StartDate,
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList() ?? new List<WorkExperienceDto>(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new ProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList() ?? new List<ProjectExperienceDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil i GetCvProfileForUser for bruker {UserId}", userId);
            throw;
        }
    }


    public async Task<List<CvDto>> GetAllCvDtos()
    {
        try
        {
            _logger.LogInformation("Henter alle CV-er fra databasen");

            var cvs = await _CvRepository.GetAllCvs();

            if (cvs == null || !cvs.Any())
            {
                _logger.LogWarning("Ingen CV-er funnet i databasen");
                return new List<CvDto>();
            }

            var cvDtos = cvs.Select(cv => new CvDto
            {
                Id = cv.Id,
                Personalia = cv.Personalia,
                DateOfBirth = cv.DateOfBirth,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? string.Empty,

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

            return cvDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil i GetAllCvDtos");
            return new List<CvDto>();
        }
    }


    public async Task<CvDto?> GetCvDtoById(string id)
    {
        try
        {
            _logger.LogInformation("Henter CV med ID {CvId}", id);

            var cv = await GetCvById(id);
            if (cv == null)
            {
                _logger.LogWarning("Fant ingen CV med ID {CvId}", id);
                return null;
            }

            _logger.LogInformation("Fant CV med ID {CvId} for bruker {UserId}", cv.Id, cv.UserId);

            return new CvDto
            {
                Id = cv.Id,
                Personalia = cv.Personalia,
                DateOfBirth = cv.DateOfBirth,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? string.Empty,

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil i GetCvDtoById for CV med ID {CvId}", id);
            return null;
        }
    }


    public async Task<Cv> CreateCvFromDto(string userId, CreateCvDto dto)
    {
        try
        {
            _logger.LogInformation("Starter opprettelse av ny CV for bruker {UserId}", userId);

            var newCv = new Cv
            {
                Personalia = dto.Personalia,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                UserId = userId.ToLower(),

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
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList() ?? new List<Certification>(),

                Courses = dto.Courses?.Select(c => new Course
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
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
                _logger.LogDebug("Behandler {Count} arbeidserfaringer", dto.WorkExperiences.Count);
                foreach (var w in dto.WorkExperiences)
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
                        var tag = await GetOrCreateTagAsync(tagValue);
                        work.Tags.Add(new WorkExperienceTag
                        {
                            Tag = tag,
                            TagId = tag.Id
                        });
                    }

                    newCv.WorkExperiences.Add(work);
                }
            }

            if (dto.ProjectExperiences != null)
            {
                _logger.LogDebug("Behandler {Count} prosjektopplevelser", dto.ProjectExperiences.Count);
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
                        var tag = await GetOrCreateTagAsync(tagValue);
                        project.Tags.Add(new ProjectExperienceTag
                        {
                            Tag = tag,
                            TagId = tag.Id
                        });
                    }

                    newCv.ProjectExperiences.Add(project);
                }
            }

            _logger.LogInformation("Lagrer ny CV for bruker {UserId}", userId);
            
            await _CvRepository.AddCv(newCv);

            _logger.LogInformation("Ny CV opprettet med ID {CvId}", newCv.Id);
            return newCv;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved opprettelse av CV for bruker {UserId}", userId);
            throw;
        }
    }


    public async Task UpdateCvFromDto(Cv existingCv, UpdateCvDto dto)
    {
        try
        {
            _logger.LogInformation("Starter oppdatering av CV-ID {CvId}", existingCv.Id);

            existingCv.Personalia = dto.Personalia;
            existingCv.DateOfBirth = dto.DateOfBirth;
            existingCv.PhoneNumber = dto.PhoneNumber;

            var allTags = new Dictionary<string, Tag>();
            var allTagValues = (dto.WorkExperiences?.SelectMany(w => w.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Concat(dto.ProjectExperiences?.SelectMany(p => p.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Distinct();

            foreach (var tagValue in allTagValues)
            {
                var tag = await _CvRepository.GetOrCreateTagAsync(tagValue);
                allTags[tagValue] = tag;
            }

            existingCv.WorkExperiences = dto.WorkExperiences?.Select(w => new WorkExperience
            {
                CompanyName = w.Company,
                Position = w.Position,
                WorkExperienceDescription = w.WorkExperienceDescription,
                StartDate = w.From,
                EndDate = w.To,
                CvId = existingCv.Id,
                Tags = w.Tags?.Select(tag => new WorkExperienceTag
                {
                    TagId = allTags[tag].Id,
                    Tag = allTags[tag]
                }).ToList() ?? new List<WorkExperienceTag>()
            }).ToList() ?? new List<WorkExperience>();

            existingCv.Educations = dto.Educations?.Select(e => new Education
            {
                School = e.School,
                Degree = e.Degree,
                EducationDescription = e.EducationDescription,
                StartDate = e.StartYear,
                EndDate = e.EndYear,
                CvId = existingCv.Id
            }).ToList() ?? new List<Education>();

            existingCv.Awards = dto.Awards?.Select(a => new Award
            {
                Name = a.Name,
                AwardDescription = a.AwardDescription,
                Organization = a.Organization,
                Year = a.Year,
                CvId = existingCv.Id
            }).ToList() ?? new List<Award>();

            existingCv.Certifications = dto.Certifications?.Select(c => new Certification
            {
                Name = c.Name,
                IssuedBy = c.IssuedBy,
                CertificationDescription = c.CertificationDescription,
                Date = c.Date,
                CvId = existingCv.Id
            }).ToList() ?? new List<Certification>();

            existingCv.Courses = dto.Courses?.Select(c => new Course
            {
                Name = c.Name,
                Provider = c.Provider,
                CourseDescription = c.CourseDescription,
                Date = c.CompletionDate,
                CvId = existingCv.Id
            }).ToList() ?? new List<Course>();

            existingCv.CompetenceOverviews = dto.CompetenceOverviews?.Select(c => new CompetenceOverview
            {
                skill_name = c.Area,
                skill_level = c.Level,
                CvId = existingCv.Id
            }).ToList() ?? new List<CompetenceOverview>();

            existingCv.Languages = dto.Languages?.Select(l => new Language
            {
                Name = l.Name,
                Proficiency = l.Proficiency,
                CvId = existingCv.Id
            }).ToList() ?? new List<Language>();

            existingCv.ProjectExperiences = dto.ProjectExperiences?.Select(p => new ProjectExperience
            {
                ProjectName = p.ProjectName,
                ProjectExperienceDescription = p.ProjectExperienceDescription,
                CompanyName = p.CompanyName,
                Role = p.Role,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                CvId = existingCv.Id,
                Tags = p.Tags?.Select(tag => new ProjectExperienceTag
                {
                    TagId = allTags[tag].Id,
                    Tag = allTags[tag]
                }).ToList() ?? new List<ProjectExperienceTag>()
            }).ToList() ?? new List<ProjectExperience>();

            existingCv.RoleOverviews = dto.RoleOverviews?.Select(r => new RoleOverview
            {
                Role = r.Role,
                RoleDescription = r.RoleDescription,
                CvId = existingCv.Id
            }).ToList() ?? new List<RoleOverview>();

            await _CvRepository.UpdateCv(existingCv);
            
            _logger.LogInformation("✅ CV-ID {CvId} ble oppdatert", existingCv.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ Feil i UpdateCvFromDto for CV-ID {CvId}", existingCv.Id);
            throw;
        }
    }

    
   public async Task<List<CvDto>> SearchCvsByKeywords(List<string> keywords)  
    {
        try
        {
            var cvs = await _CvRepository.SearchCvsByKeywords(keywords);

            _logger.LogInformation("Søk utført med {KeywordCount} søkeord. Fant {ResultCount} CV-er.",
                keywords?.Count ?? 0,
                cvs?.Count ?? 0);

            return cvs.Select(cv => new CvDto
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil under søk etter CV-er med søkeord: {Keywords}", string.Join(", ", keywords));
            return new List<CvDto>(); // returnerer tomt resultat for å ikke kræsje frontend
        }
    }

}