namespace CvAPI2.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public required string Company { get; set; }
        public required string Position { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string? CvId { get; set; }  // Fremmednøkkel
        public Cv? Cv { get; set; }       // Navigasjonsproperty
    }
}