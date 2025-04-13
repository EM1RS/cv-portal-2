using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
namespace CvAPI2.DTO
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string? CvId { get; set; }     

    }

}