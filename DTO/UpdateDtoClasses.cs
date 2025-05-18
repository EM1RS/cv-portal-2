public class UpdateEducationDto : EducationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateWorkExperienceDto : WorkExperienceDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateAwardDto : AwardDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateCertificationDto : CertificationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateCourseDto : CourseDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateCompetenceOverviewDto : CompetenceOverviewDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateLanguageDto : LanguageDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateProjectExperienceDto : ProjectExperienceDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public class UpdateRoleOverviewDto : RoleOverviewDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
