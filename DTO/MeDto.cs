public class MeSummaryDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
}

public class MeProfileDto
{   
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}

public class MeEditDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}

public class CvProfileDto
{
    public string Id { get; set; }
    public string Personalia { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int PhoneNumber { get; set; }

    public List<EducationDto> Educations { get; set; }
    public List<AwardDto> Awards { get; set; }
    public List<CertificationDto> Certifications { get; set; }
    public List<CourseDto> Courses { get; set; }
    public List<CompetenceOverviewDto> CompetenceOverviews { get; set; }
    public List<LanguageDto> Languages { get; set; }
    public List<RoleOverviewDto> RoleOverviews { get; set; }
    public List<WorkExperienceDto> WorkExperiences { get; set; }
    public List<ProjectExperienceDto> ProjectExperiences { get; set; }
}
