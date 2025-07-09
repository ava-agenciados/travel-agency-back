using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    /// <summary>
    /// DTO utilizado para realizar o login do usuário.
    /// Contém as credenciais básicas necessárias: e-mail e senha.
    /// </summary>
    public record LoginUserRequestDTO
    {
        /// <summary>
        /// E-mail do usuário.
        /// É obrigatório e deve estar presente no corpo da requisição.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; init; }

        /// <summary>
        /// Senha do usuário.
        /// Campo obrigatório para autenticação.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; init; }
    }
}
