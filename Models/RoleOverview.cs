namespace CvAPI2.Models
{
    public class RoleOverview
    {
        public int Id { get; set; }
        public string? Role { get; set; }
        public string? RoleDescription { get; set; }
   	    public string? CvId { get; set; }    // Fremmednøkkel
    	public Cv? Cv { get; set; }     // Navigasjonsproperty
    }
}