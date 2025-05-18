using CvAPI2.Models;
using CvAPI2.Models.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;



public class CvService : ICvService

{
    private readonly ICvRepository _CvRepository;
    private readonly ILogger<CvService> _logger;
    private readonly string _openAiKey;

    public CvService(ICvRepository CvRepository, ILogger<CvService> logger, IOptions<OpenAiSettings> settings)
    {
        _logger = logger;
        _CvRepository = CvRepository;
        _openAiKey = settings.Value.ApiKey;
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
                ProfileImageUrl = cv.ProfileImageUrl,
                PhoneNumber = cv.PhoneNumber,

                Educations = cv.Educations?.Select(e => new UpdateEducationDto
                {   
                    Id = e.Id,
                    School = e.School,
                    Degree = e.Degree,
                    StudyName = e.StudyName,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList() ?? new List<UpdateEducationDto>(),

                Awards = cv.Awards?.Select(a => new UpdateAwardDto
                {   
                    Id = a.Id,
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList() ?? new List<UpdateAwardDto>(),

                Certifications = cv.Certifications?.Select(c => new UpdateCertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IssuedBy = c.IssuedBy,
                    CertificationDescription = c.CertificationDescription,
                    Date = c.Date
                }).ToList() ?? new List<UpdateCertificationDto>(),

                Courses = cv.Courses?.Select(c => new UpdateCourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Provider = c.Provider,
                    CourseDescription = c.CourseDescription,
                    CompletionDate = c.Date
                }).ToList() ?? new List<UpdateCourseDto>(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(co => new UpdateCompetenceOverviewDto
                {
                    Id = co.Id,
                    Area = co.skill_name,
                    Level = co.skill_level
                }).ToList() ?? new List<UpdateCompetenceOverviewDto>(),

                Languages = cv.Languages?.Select(l => new UpdateLanguageDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList() ?? new List<UpdateLanguageDto>(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new UpdateRoleOverviewDto
                {
                    Id = r.Id,
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                }).ToList() ?? new List<UpdateRoleOverviewDto>(),

                WorkExperiences = cv.WorkExperiences?.Select(w => new UpdateWorkExperienceDto
                {
                    Id = w.Id,
                    CompanyName = w.CompanyName,
                    Position = w.Position,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    From = w.StartDate,
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList() ?? new List<UpdateWorkExperienceDto>(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new UpdateProjectExperienceDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList() ?? new List<UpdateProjectExperienceDto>()
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
                ProfileImageUrl = cv.ProfileImageUrl,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? string.Empty,

                WorkExperiences = cv.WorkExperiences?.Select(w => new UpdateWorkExperienceDto
                {
                    Id = w.Id,
                    CompanyName = w.CompanyName,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                Educations = cv.Educations?.Select(e => new UpdateEducationDto
                {
                    Id = e.Id,
                    School = e.School,
                    Degree = e.Degree,
                    StudyName = e.StudyName,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new UpdateAwardDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new UpdateCertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new UpdateCourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
                    Provider = c.Provider,
                    CompletionDate = c.Date
                }).ToList(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(c => new UpdateCompetenceOverviewDto
                {
                    Id = c.Id,
                    Area = c.skill_name,
                    Level = c.skill_level
                }).ToList(),

                Languages = cv.Languages?.Select(l => new UpdateLanguageDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new UpdateProjectExperienceDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new UpdateRoleOverviewDto
                {
                    Id = r.Id,
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
                ProfileImageUrl = cv.ProfileImageUrl,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? string.Empty,

                WorkExperiences = cv.WorkExperiences?.Select(w => new UpdateWorkExperienceDto
                {
                    Id = w.Id,
                    CompanyName = w.CompanyName,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                Educations = cv.Educations?.Select(e => new UpdateEducationDto
                {
                    Id = e.Id,
                    School = e.School,
                    Degree = e.Degree,
                    StudyName = e.StudyName,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new UpdateAwardDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Organization = a.Organization,
                    AwardDescription = a.AwardDescription,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new UpdateCertificationDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new UpdateCourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
                    Provider = c.Provider,
                    CompletionDate = c.Date
                }).ToList(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(c => new UpdateCompetenceOverviewDto
                {
                    Id = c.Id,
                    Area = c.skill_name,
                    Level = c.skill_level
                }).ToList(),

                Languages = cv.Languages?.Select(l => new UpdateLanguageDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new UpdateProjectExperienceDto
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>()
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new UpdateRoleOverviewDto
                {
                    Id = r.Id,
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
            _logger.LogInformation("Oppretter ny CV for bruker {UserId}", userId);

            // Samle unike tags først for ytelse
            var allTagValues = (dto.WorkExperiences?.SelectMany(w => w.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Concat(dto.ProjectExperiences?.SelectMany(p => p.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Distinct();

            var allTags = new Dictionary<string, Tag>();
            foreach (var tagValue in allTagValues)
            {
                var tag = await GetOrCreateTagAsync(tagValue);
                allTags[tagValue] = tag;
            }

            var newCv = new Cv
            {
                UserId = userId.ToLower(),
                Personalia = dto.Personalia,
                DateOfBirth = dto.DateOfBirth,
                ProfileImageUrl = dto.ProfileImageUrl,
                PhoneNumber = dto.PhoneNumber,

                Educations = dto.Educations?.Select(e => new Education
                {
                    School = e.School,
                    Degree = e.Degree,
                    StudyName = e.StudyName,
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

                WorkExperiences = dto.WorkExperiences?.Select(w => new WorkExperience
                {
                    CompanyName = w.CompanyName,
                    Position = w.Position,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    StartDate = w.From,
                    EndDate = w.To,
                    Tags = w.Tags?.Where(tag => allTags.ContainsKey(tag)).Select(tag => new WorkExperienceTag
                    {
                        Tag = allTags[tag],
                        TagId = allTags[tag].Id
                    }).ToList() ?? new List<WorkExperienceTag>()
                }).ToList() ?? new List<WorkExperience>(),

                ProjectExperiences = dto.ProjectExperiences?.Select(p => new ProjectExperience
                {
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tags = p.Tags?.Where(tag => allTags.ContainsKey(tag)).Select(tag => new ProjectExperienceTag
                    {
                        Tag = allTags[tag],
                        TagId = allTags[tag].Id
                    }).ToList() ?? new List<ProjectExperienceTag>()
                }).ToList() ?? new List<ProjectExperience>()
            };

            await _CvRepository.AddCv(newCv);

            _logger.LogInformation("Ny CV opprettet med ID {CvId} for bruker {UserId}", newCv.Id, userId);
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
            _logger.LogInformation("🔄 Starter oppdatering av CV-ID {CvId}", existingCv.Id);

            if (dto.Personalia != null)
                existingCv.Personalia = dto.Personalia;

            if (dto.DateOfBirth != null)
                existingCv.DateOfBirth = dto.DateOfBirth;

            if (dto.ProfileImageUrl != null)
                existingCv.ProfileImageUrl = dto.ProfileImageUrl;

            if (dto.PhoneNumber != null)
                existingCv.PhoneNumber = dto.PhoneNumber;

            // Samle alle unike tags
            var allTags = new Dictionary<string, Tag>();
            var allTagValues = (dto.WorkExperiences?.SelectMany(w => w.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Concat(dto.ProjectExperiences?.SelectMany(p => p.Tags ?? new List<string>()) ?? Enumerable.Empty<string>())
                .Distinct();

            foreach (var tagValue in allTagValues)
            {
                var tag = await _CvRepository.GetOrCreateTagAsync(tagValue);
                allTags[tagValue] = tag;
            }

            if (dto.WorkExperiences != null)
            {
                var updatedWorkExperiences = new List<WorkExperience>();

                foreach (var w in dto.WorkExperiences)
                {
                    var existing = existingCv.WorkExperiences?.FirstOrDefault(x => x.Id == w.Id);

                    if (existing != null)
                    {   existing.Id = w.Id;
                        existing.CompanyName = w.CompanyName;
                        existing.Position = w.Position;
                        existing.WorkExperienceDescription = w.WorkExperienceDescription;
                        existing.StartDate = w.From;
                        existing.EndDate = w.To;

                        existing.Tags = w.Tags?.Where(tag => allTags.ContainsKey(tag)).Select(tag => new WorkExperienceTag
                        {
                            Tag = allTags[tag],
                            TagId = allTags[tag].Id
                        }).ToList() ?? new List<WorkExperienceTag>();

                        updatedWorkExperiences.Add(existing);
                    }
                    else
                    {
                        updatedWorkExperiences.Add(new WorkExperience
                        {
                            Id = w.Id,
                            CompanyName = w.CompanyName,
                            Position = w.Position,
                            WorkExperienceDescription = w.WorkExperienceDescription,
                            StartDate = w.From,
                            EndDate = w.To,
                            CvId = existingCv.Id,
                            Tags = w.Tags?.Where(tag => allTags.ContainsKey(tag)).Select(tag => new WorkExperienceTag
                            {
                                Tag = allTags[tag],
                                TagId = allTags[tag].Id
                            }).ToList() ?? new List<WorkExperienceTag>()
                        });
                    }
                }

                existingCv.WorkExperiences = updatedWorkExperiences;
            }

            if (dto.Educations != null)
            {
            var updatedEducations = new List<Education>();

            foreach (var eduDto in dto.Educations)
            {
                var existing = existingCv.Educations?.FirstOrDefault(e => e.Id == eduDto.Id);

                if (existing != null)
                {
                        // Oppdater eksisterende utdanning
                    existing.Id = eduDto.Id;
                    existing.School = eduDto.School;
                    existing.Degree = eduDto.Degree;
                    existing.StudyName = eduDto.StudyName;
                    existing.EducationDescription = eduDto.EducationDescription;
                    existing.StartDate = eduDto.StartYear;
                    existing.EndDate = eduDto.EndYear;

                    updatedEducations.Add(existing);
                }
                else
                {
                    // Ny utdanning
                    updatedEducations.Add(new Education
                    {
                        Id = eduDto.Id,
                        School = eduDto.School,
                        Degree = eduDto.Degree,
                        StudyName = eduDto.StudyName,
                        EducationDescription = eduDto.EducationDescription,
                        StartDate = eduDto.StartYear,
                        EndDate = eduDto.EndYear,
                        CvId = existingCv.Id
                    });
                }
            }

            existingCv.Educations = updatedEducations;
             }

            if (dto.Awards != null)
            {
                var updatedAwards = new List<Award>();

                foreach (var a in dto.Awards)
                {
                    var existing = existingCv.Awards?.FirstOrDefault(x => x.Id == a.Id);
                    if (existing != null)
                    {
                        existing.Id = a.Id;
                        existing.Name = a.Name;
                        existing.AwardDescription = a.AwardDescription;
                        existing.Organization = a.Organization;
                        existing.Year = a.Year;

                        updatedAwards.Add(existing);
                    }
                    else
                    {
                        updatedAwards.Add(new Award
                        {
                            Id = a.Id,
                            Name = a.Name,
                            AwardDescription = a.AwardDescription,
                            Organization = a.Organization,
                            Year = a.Year,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.Awards = updatedAwards;
            }

            if (dto.Certifications != null)
            {
                var updatedCerts = new List<Certification>();

                foreach (var c in dto.Certifications)
                {
                    var existing = existingCv.Certifications?.FirstOrDefault(x => x.Id == c.Id);
                    if (existing != null)
                    {   
                        existing.Id = c.Id;
                        existing.Name = c.Name;
                        existing.IssuedBy = c.IssuedBy;
                        existing.CertificationDescription = c.CertificationDescription;
                        existing.Date = c.Date;

                        updatedCerts.Add(existing);
                    }
                    else
                    {
                        updatedCerts.Add(new Certification
                        {
                            Id = c.Id,
                            Name = c.Name,
                            IssuedBy = c.IssuedBy,
                            CertificationDescription = c.CertificationDescription,
                            Date = c.Date,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.Certifications = updatedCerts;
            }

            if (dto.Courses != null)
            {
                var updatedCourses = new List<Course>();

                foreach (var c in dto.Courses)
                {
                    var existing = existingCv.Courses?.FirstOrDefault(x => x.Id == c.Id);
                    if (existing != null)
                    {
                        existing.Id = c.Id;
                        existing.Name = c.Name;
                        existing.Provider = c.Provider;
                        existing.CourseDescription = c.CourseDescription;
                        existing.Date = c.CompletionDate;

                        updatedCourses.Add(existing);
                    }
                    else
                    {
                        updatedCourses.Add(new Course
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Provider = c.Provider,
                            CourseDescription = c.CourseDescription,
                            Date = c.CompletionDate,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.Courses = updatedCourses;
            }

            if (dto.CompetenceOverviews != null)
            {
                var updatedCompetences = new List<CompetenceOverview>();

                foreach (var c in dto.CompetenceOverviews)
                {
                    var existing = existingCv.CompetenceOverviews?.FirstOrDefault(x => x.Id == c.Id);
                    if (existing != null)
                    {
                        existing.Id = c.Id;
                        existing.skill_name = c.Area;
                        existing.skill_level = c.Level;

                        updatedCompetences.Add(existing);
                    }
                    else
                    {
                        updatedCompetences.Add(new CompetenceOverview
                        {   
                            Id = c.Id,
                            skill_name = c.Area,
                            skill_level = c.Level,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.CompetenceOverviews = updatedCompetences;
            }

            if (dto.Languages != null)
            {
                var updatedLanguages = new List<Language>();

                foreach (var l in dto.Languages)
                {
                    var existing = existingCv.Languages?.FirstOrDefault(x => x.Id == l.Id);
                    if (existing != null)
                    {
                        existing.Id = l.Id;
                        existing.Name = l.Name;
                        existing.Proficiency = l.Proficiency;

                        updatedLanguages.Add(existing);
                    }
                    else
                    {
                        updatedLanguages.Add(new Language
                        {
                            Id = l.Id,
                            Name = l.Name,
                            Proficiency = l.Proficiency,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.Languages = updatedLanguages;
            }


            if (dto.ProjectExperiences != null)
            {
                var updatedProjects = new List<ProjectExperience>();

                foreach (var p in dto.ProjectExperiences)
                {
                    var existing = existingCv.ProjectExperiences?.FirstOrDefault(x => x.Id == p.Id);

                    if (existing != null)
                    {
                        existing.Id = p.Id;
                        existing.ProjectName = p.ProjectName;
                        existing.CompanyName = p.CompanyName;
                        existing.ProjectExperienceDescription = p.ProjectExperienceDescription;
                        existing.Role = p.Role;
                        existing.StartDate = p.StartDate;
                        existing.EndDate = p.EndDate;

                        existing.Tags = p.Tags?.Where(tag => allTags.ContainsKey(tag)).Select(tag => new ProjectExperienceTag
                        {
                            Tag = allTags[tag],
                            TagId = allTags[tag].Id
                        }).ToList() ?? new List<ProjectExperienceTag>();

                        updatedProjects.Add(existing);
                    }
                    else
                    {
                        
                        updatedProjects.Add(new ProjectExperience
                        {
                            Id = p.Id,
                            ProjectName = p.ProjectName,
                            CompanyName = p.CompanyName,
                            ProjectExperienceDescription = p.ProjectExperienceDescription,
                            Role = p.Role,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            CvId = existingCv.Id,
                            Tags = p.Tags?.Select(t => new ProjectExperienceTag
                            {
                                TagId = allTags[t].Id,
                                Tag = allTags[t]
                            }).ToList() ?? new List<ProjectExperienceTag>()
                        });
                    }
                }

                existingCv.ProjectExperiences = updatedProjects;
            }


            if (dto.RoleOverviews != null)
            {
                var updatedRoles = new List<RoleOverview>();

                foreach (var r in dto.RoleOverviews)
                {
                    var existing = existingCv.RoleOverviews?.FirstOrDefault(x => x.Id == r.Id);

                    if (existing != null)
                    {
                        existing.Role = r.Role;
                        existing.RoleDescription = r.RoleDescription;
                        updatedRoles.Add(existing);
                    }
                    else
                    {
                        updatedRoles.Add(new RoleOverview
                        {
                            Id = r.Id,
                            Role = r.Role,
                            RoleDescription = r.RoleDescription,
                            CvId = existingCv.Id
                        });
                    }
                }

                existingCv.RoleOverviews = updatedRoles;
            }

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
                ProfileImageUrl = cv.ProfileImageUrl,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,
                FullName = cv.User?.FullName ?? "",

                WorkExperiences = cv.WorkExperiences?.Select(w => new UpdateWorkExperienceDto
                {
                    CompanyName = w.CompanyName,
                    WorkExperienceDescription = w.WorkExperienceDescription,
                    Tags = w.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>(),
                    Position = w.Position,
                    From = w.StartDate,
                    To = w.EndDate
                }).ToList(),

                Educations = cv.Educations?.Select(e => new UpdateEducationDto
                {
                    School = e.School,
                    Degree = e.Degree,
                    StudyName = e.StudyName,
                    EducationDescription = e.EducationDescription,
                    StartYear = e.StartDate,
                    EndYear = e.EndDate
                }).ToList(),

                Awards = cv.Awards?.Select(a => new UpdateAwardDto
                {
                    Name = a.Name,
                    AwardDescription = a.AwardDescription,
                    Organization = a.Organization,
                    Year = a.Year
                }).ToList(),

                Certifications = cv.Certifications?.Select(c => new UpdateCertificationDto
                {
                    Name = c.Name,
                    CertificationDescription = c.CertificationDescription,
                    IssuedBy = c.IssuedBy,
                    Date = c.Date
                }).ToList(),

                Courses = cv.Courses?.Select(c => new UpdateCourseDto
                {
                    Name = c.Name,
                    CourseDescription = c.CourseDescription,
                    Provider = c.Provider,
                    CompletionDate = c.Date
                }).ToList(),

                CompetenceOverviews = cv.CompetenceOverviews?.Select(c => new UpdateCompetenceOverviewDto
                {
                    Area = c.skill_name,
                    Level = c.skill_level
                }).ToList(),

                Languages = cv.Languages?.Select(l => new UpdateLanguageDto
                {
                    Name = l.Name,
                    Proficiency = l.Proficiency
                }).ToList(),

                ProjectExperiences = cv.ProjectExperiences?.Select(p => new UpdateProjectExperienceDto
                {
                    ProjectName = p.ProjectName,
                    CompanyName = p.CompanyName,
                    ProjectExperienceDescription = p.ProjectExperienceDescription,
                    Tags = p.Tags?.Select(t => t.Tag.Value).ToList() ?? new List<string>(),
                    Role = p.Role,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList(),

                RoleOverviews = cv.RoleOverviews?.Select(r => new UpdateRoleOverviewDto
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