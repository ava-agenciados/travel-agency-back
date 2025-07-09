using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    /// <summary>
    /// DTO (Data Transfer Object) usado para iniciar o processo de recuperação de senha.
    /// Contém apenas o e-mail do usuário que deseja redefinir sua senha.
    /// </summary>
    public class ForgotPasswordRequestDTO
    {
        /// <summary>
        /// E-mail do usuário que solicitou a redefinição de senha.
        /// Deve estar em formato válido e é obrigatório.
        /// </summary>
        [Required] // Validação obrigatória: o campo não pode ser nulo ou vazio
        [EmailAddress(ErrorMessage = "Invalid email format.")]  // Valida se o e-mail está no formato correto
        public string Email { get; init; } // Propriedade somente leitura na inicialização (init-only)
    }
}
