namespace CvAPI2.Models
{
    public class Language
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Name { get; set; } = string.Empty;
        public string Proficiency { get; set; } = string.Empty;
        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    }

}