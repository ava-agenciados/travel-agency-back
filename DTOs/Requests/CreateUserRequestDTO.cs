using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    /// <summary>
    /// DTO utilizado para receber os dados necessários no processo de registro de um novo usuário na aplicação.
    /// 
    /// Esta classe define os campos obrigatórios para cadastro, incluindo validações de formato e obrigatoriedade.
    /// É utilizada como parâmetro de entrada nos endpoints de registro, garantindo que apenas informações válidas
    /// sejam processadas pelo sistema.
    /// 
    /// Propriedades:
    /// - FirstName: Nome do usuário (obrigatório).
    /// - LastName: Sobrenome do usuário (obrigatório).
    /// - CPFPassport: Documento de identificação, podendo ser CPF ou Passaporte (obrigatório).
    /// - Email: E-mail do usuário, validado quanto ao formato e obrigatoriedade.
    /// - Password: Senha do usuário, com exigência de no mínimo 8 caracteres.
    /// 
    /// As anotações de data annotation garantem a validação automática dos campos durante o binding do modelo.
    /// </summary>
    public record CreateUserRequestDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; init; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; init; }

        [Required(ErrorMessage = "CPF or Passport is required.")]
        public string CPFPassport { get; init; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; init; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; init; }
    }
}
