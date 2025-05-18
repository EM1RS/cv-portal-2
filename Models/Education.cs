namespace CvAPI2.Models
{
    public class Education
    {
        public string  Id { get; set; } = Guid.NewGuid().ToString();
        public string? School { get; set; }
        public string? Degree { get; set; }
        public string? StudyName { get; set; }
        public string EducationDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    }
}