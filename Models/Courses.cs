using CvAPI2.Models;

public class Course
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string CourseDescription { get; set; }
    public string Provider { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public string? CvId { get; set; }           // Fremmednøkkel
    public Cv? Cv { get; set; }     // Navigasjonsproperty
    
}
