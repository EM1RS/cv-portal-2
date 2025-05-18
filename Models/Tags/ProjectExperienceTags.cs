using CvAPI2.Models;
using CvAPI2.Models.Tag;

public class ProjectExperienceTag
{
    public string ProjectExperienceId { get; set; } = Guid.NewGuid().ToString();
    public ProjectExperience ProjectExperience { get; set; } = null!;

    public string TagId { get; set; } = Guid.NewGuid().ToString();
    public Tag Tag { get; set; } = null!;
}