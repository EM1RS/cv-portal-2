namespace CvAPI2.Models
{
    public class Award
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string AwardDescription { get; set; }
        public int Year { get; set; }

        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    }

}