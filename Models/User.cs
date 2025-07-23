using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.Models
{
    public class User : IdentityUser<int>
    {
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // Regex para validar CPF (11 dígitos) ou Passaporte (2 letras seguidas de 6 a 9 dígitos)
        //se tiver 6 dígitos, é considerado um passaporte
        //se tiver 11 dígitos, é considerado um CPF
        [Required]
        [MinLength(6)]
        [MaxLength(11)]
        [RegularExpression(@"^\d{11}$|^[A-Z]{2}\d{6}$", ErrorMessage = "CPF ou número de passaporte inválidos")]
        public string CPFPassport { get; set; }

        //Padrão todo usuario é um cliente
        [Required]
        public string Role { get; set; } = "Cliente"; // Default para Cliente


        public UserDocument UserDocument { get; set; } // Navegação para o documento do usuário
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Rating> Ratings { get; set; } // Coleção de avaliações feitas pelo usuário

    }
}
