namespace CvAPI2.Models
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CertificationDescription { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    
    }
}
