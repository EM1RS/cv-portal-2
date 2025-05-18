using CvAPI2.Models;
using CvAPI2.Models.Tag;

public class WorkExperienceTag
{
    public string WorkExperienceId { get; set; } = Guid.NewGuid().ToString();
    public WorkExperience WorkExperience { get; set; } = null!;

    public string TagId { get; set; } = Guid.NewGuid().ToString();
    public Tag Tag { get; set; } = null!;
}