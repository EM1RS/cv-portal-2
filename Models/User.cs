using Microsoft.AspNetCore.Identity;
namespace CvAPI2.Models
{
    public class User : IdentityUser
    {
        public required string FullName { get; set; }
        public Cv Cv { get; set; }

    }

    public class Role : IdentityRole
    {
        public ICollection<User> Users{ get; set; }
    }

}