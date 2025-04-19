namespace CvAPI2.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public required string CompanyName { get; set; }
        public string WorkExperienceDescription { get; set; }
        public required string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? CvId { get; set; }  // Fremmednøkkel
        public Cv? Cv { get; set; }       // Navigasjonsproperty

        public ICollection<WorkExperienceTag> Tags { get; set; } = new List<WorkExperienceTag>();

    }
}