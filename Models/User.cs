using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string CPFPassport { get; set; }

    }
}
