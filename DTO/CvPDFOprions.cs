public class CvPdfOptions
{
    public bool IncludeEducations { get; set; }
    public bool IncludeLanguages { get; set; } 
    public bool IncludeAwards { get; set; } 
    public bool IncludeCertifications { get; set; }
    public bool IncludeCourses { get; set; } 
    public bool IncludeCompetenceOverviews { get; set; }
    public bool IncludeRoleOverviews { get; set; } 

    public List<string> WorkExperienceIds { get; set; } = new();
    public List<string> ProjectExperienceIds { get; set; } = new();
}
