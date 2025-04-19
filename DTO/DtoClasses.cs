public class EducationDto
{
    public string School { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string EducationDescription { get; set; } = string.Empty;
    public DateTime StartYear { get; set; }
    public DateTime EndYear { get; set; }
}

public class WorkExperienceDto
{
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string WorkExperienceDescription { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class AwardDto {
    public string Name { get; set; } = string.Empty;
    public string AwardDescription { get; set; } = string.Empty;

    public string Organization { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class CertificationDto {
    public string Name { get; set; } = string.Empty;
    public string CertificationDescription { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class CourseDto
{
    public string Name { get; set; } = string.Empty;
    public string CourseDescription { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
}

public class CompetenceOverviewDto
{
    public string Area { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class LanguageDto
{
    public string Name { get; set; } = string.Empty;
    public string Proficiency { get; set; } = string.Empty;
}

public class ProjectExperienceDto
{
    public string ProjectName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ProjectExperienceDescription { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class RoleOverviewDto
{
    public string Role { get; set; } = string.Empty;
    public string RoleDescription { get; set; } = string.Empty;
}


