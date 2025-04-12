namespace CvAPI2.Models
{
    public class WorkExperience
    {
        public int Id { get; set; }
        public required string Company { get; set; }
        public required string Role { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public int CvId { get; set; }  // Fremmednøkkel
        public required Cv Cv { get; set; }       // Navigasjonsproperty
    }
}