using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.Models
{
    public class User
    {
        [Required]
        private string _firstName;

        [Required]
        private string _lastName;

        [Required]
        [EmailAddress]
        private string _email;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        private string _password;
    }
}
