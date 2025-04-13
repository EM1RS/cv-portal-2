public class EducationDto
{
    public string School { get; set; }
    public string Degree { get; set; }
    public DateTime StartYear { get; set; }
    public DateTime EndYear { get; set; }
}

public class WorkExperienceDto
{
    public string Company { get; set; }
    public string Position { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class AwardDto {
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
}

public class CertificationDto {
    public string Name { get; set; }
    public string IssuedBy { get; set; }
    public DateTime Date { get; set; }
}

public class CourseDto
{
    public string Name { get; set; }
    public string Provider { get; set; }
    public DateTime CompletionDate { get; set; }
}

public class CompetenceOverviewDto
{
    public string Area { get; set; }
    public string Level { get; set; }
}

public class LanguageDto
{
    public string Name { get; set; }
    public string Proficiency { get; set; }
}

public class PositionDto
{
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ProjectExperienceDto
{
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public string Role { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class RoleOverviewDto
{
    public string Role { get; set; }
    public string Description { get; set; }
}


