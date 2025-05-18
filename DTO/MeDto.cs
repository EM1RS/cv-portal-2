public class MeSummaryDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class MeProfileDto
{   
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    
}

public class MeEditDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}

public class CvProfileDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Personalia { get; set; }
    public string? ProfileImageUrl { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }

    public List<UpdateEducationDto> Educations { get; set; }
    public List<UpdateAwardDto> Awards { get; set; }
    public List<UpdateCertificationDto> Certifications { get; set; }
    public List<UpdateCourseDto> Courses { get; set; }
    public List<UpdateCompetenceOverviewDto> CompetenceOverviews { get; set; }
    public List<UpdateLanguageDto> Languages { get; set; }
    public List<UpdateRoleOverviewDto> RoleOverviews { get; set; }
    public List<UpdateWorkExperienceDto> WorkExperiences { get; set; }
    public List<UpdateProjectExperienceDto> ProjectExperiences { get; set; }
}
