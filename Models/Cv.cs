using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CvAPI2.Models
{
    public class Cv
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); 
        public string Personalia { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int PhoneNumber { get; set; }
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }  

        public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public ICollection<Education>? Educations { get; set; } 
        public ICollection<Award>? Awards { get; set; } 
        public ICollection<Certification>? Certifications { get; set; }
        public ICollection<Course>? Courses { get; set; }  
        public ICollection<CompetenceOverview>? CompetenceOverviews { get; set; } 
        public ICollection<Language>? Languages { get; set; } 
        public ICollection<ProjectExperience> ProjectExperiences { get; set; } = new List<ProjectExperience>();
        public ICollection<RoleOverview>? RoleOverviews { get; set; } 
    }
}