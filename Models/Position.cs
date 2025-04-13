namespace CvAPI2.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    }

}