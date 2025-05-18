public class UpdateCvDto
{
    public string Personalia { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string PhoneNumber { get; set; }
    public List<UpdateWorkExperienceDto>? WorkExperiences { get; set; }
    public List<UpdateEducationDto>? Educations { get; set; }
    public List<UpdateAwardDto>? Awards { get; set; }
    public List<UpdateCertificationDto>? Certifications { get; set; }
    public List<UpdateCourseDto>? Courses { get; set; }
    public List<UpdateCompetenceOverviewDto>? CompetenceOverviews { get; set; }
    public List<UpdateLanguageDto>? Languages { get; set; }
    public List<UpdateProjectExperienceDto>? ProjectExperiences { get; set; }
    public List<UpdateRoleOverviewDto>? RoleOverviews { get; set; }
}
