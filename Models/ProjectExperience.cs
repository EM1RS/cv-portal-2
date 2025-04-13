using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CvAPI2.Models
{
    public class ProjectExperience

    {
        public int Id { get; set;}
        public string? ProjectName { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? Role { get; set; }
        public string? CvId { get; set; }           // Fremmednøkkel
        public Cv? Cv { get; set; }     // Navigasjonsproperty
    
    }
}
