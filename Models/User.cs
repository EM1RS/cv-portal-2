using Microsoft.AspNetCore.Identity;
namespace CvAPI2.Models
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
        public Cv Cv { get; set; }
        public string RoleId { get; set; }  // Fremmednøkkel til Role
        public Role? Role { get; set; }   // Navigasjonsproperty

    }

    public class Role : IdentityRole
    {
        public ICollection<User> Users{ get; set; }
    }

}