namespace CvAPI2.Models
{ 
    public class CompetenceOverview
    {
        public int id { get; set; }
        public string? skill_name { get; set; }
        public string? skill_level {get; set; }
   	    public string? CvId { get; set; }           // Fremmednøkkel
    	public Cv? Cv { get; set; }     // Navigasjonsproperty
    }
}



