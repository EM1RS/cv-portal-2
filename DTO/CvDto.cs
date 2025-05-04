using System.ComponentModel.DataAnnotations;
using CvAPI2.Models;

public class CvDto
{
    public string Id { get; set; }
    [Required]
    public string Personalia { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }  

    public string PhoneNumber { get; set; }

    public List<EducationDto>? Educations { get; set; }
    public List<WorkExperienceDto>? WorkExperiences { get; set; }
    public List<AwardDto>? Awards { get; set; }
    public List<CertificationDto>? Certifications { get; set; }
    public List<CourseDto>? Courses { get; set; }
    public List<CompetenceOverviewDto>? CompetenceOverviews { get; set; }
    public List<LanguageDto>? Languages { get; set; }
    public List<ProjectExperienceDto>? ProjectExperiences { get; set; }
    public List<RoleOverviewDto>? RoleOverviews { get; set; }
}
