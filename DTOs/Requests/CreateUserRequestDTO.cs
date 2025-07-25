using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
    /// - PhoneNumber: Número de telefone do usuário (opcional).
    /// - Email: E-mail do usuário, validado quanto ao formato e obrigatoriedade.
    /// - Password: Senha do usuário, com exigência de no mínimo 8 caracteres.
    /// - Role: Role do usuário, com valor padrão "Cliente" (obrigatório).
    /// 
    /// As anotações de data annotation garantem a validação automática dos campos durante o binding do modelo.
    /// </summary>
    public record CreateUserRequestDTO
    {
        [Required(ErrorMessage = "É necessario informar o primeiro nome!")]
        public string? FirstName { get; init; }

        [Required(ErrorMessage = "É necessario informar o sobrenome!")]
        public string? LastName { get; init; }

        [Required(ErrorMessage = "O Número de CPF ou de Passaporte precisa ser informado!")]
        public string? CPFPassport { get; init; }

        [Phone(ErrorMessage = "O Número de telefone está em um formato inválido")]
        public string? PhoneNumber { get; init; }

        [EmailAddress]
        [Required(ErrorMessage = "É Necessario um endereço de E-mail válido")]
        public string? Email { get; init; }

        [Required]
        [MinLength(8, ErrorMessage = "A senha precisa ter pelo menos 8 caracteres")]
        public string? Password { get; init; }

    }
}
