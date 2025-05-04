using System.ComponentModel.DataAnnotations;

namespace CvAPI2.DTO
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "E-postadressen er ikke gyldig")]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Passordet må være minst 6 tegn!")]
        public string Password { get; set; }

        [RegularExpression("^(Admin|User)$", ErrorMessage = "Rollen må være enten 'Admin' eller 'User'")]
        public string Role { get; set; }

    }
}