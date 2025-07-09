using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests
{
    /// <summary>
    /// DTO utilizado para redefinir a senha de um usuário.
    /// Contém a nova senha que será definida após a validação do token.
    /// </summary>
    public class NewPasswordRequestDTO
    {
        /// <summary>
        /// Nova senha que será atribuída ao usuário.
        /// Este campo é obrigatório.
        /// </summary>
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; init; } // `init` permite que o valor seja atribuído apenas na inicialização
    }
}
