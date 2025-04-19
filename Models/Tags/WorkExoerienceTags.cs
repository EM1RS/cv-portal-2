using CvAPI2.Models;
using CvAPI2.Models.Tag;

public class WorkExperienceTag
{
    public int WorkExperienceId { get; set; }
    public WorkExperience WorkExperience { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}