using CvAPI2.Models;
using CvAPI2.Models.Tag;

public class ProjectExperienceTag
{
    public int ProjectExperienceId { get; set; }
    public ProjectExperience ProjectExperience { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}