namespace CvAPI2.Models
{
    public class Cv
    {
        public int Id { get; set; }
        public string Personalia { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }  

        public ICollection<WorkExperience> WorkExperiences { get; set; } 
        public ICollection<Education> Educations { get; set; } 
    }
}