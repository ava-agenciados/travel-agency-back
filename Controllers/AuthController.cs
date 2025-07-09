using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Services;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de autenticação de usuários na API.
    /// 
    /// Esta classe expõe endpoints para registro e login de usuários, utilizando o serviço <see cref="AuthService"/>,
    /// que integra o ASP.NET Core Identity para manipulação segura de credenciais e autenticação.
    /// 
    /// Funcionalidades principais:
    /// - POST /api/v1/auth/register: Registra um novo usuário com os dados fornecidos no corpo da requisição.
    ///   Utiliza o Identity para criar o usuário e armazenar a senha de forma segura (bcrypt, conforme configurado).
    ///   Retorna sucesso ou mensagens de erro de validação.
    /// 
    /// - POST /api/v1/auth/login: Realiza a autenticação do usuário com e-mail e senha.
    ///   Utiliza o Identity para validar as credenciais e, em caso de sucesso, autentica o usuário na aplicação.
    ///   Retorna sucesso ou erro de autenticação.
    /// 
    /// Observações:
    /// - Os métodos utilizam DTOs para receber os dados de entrada.
    /// - O Identity gerencia o hash e a verificação das senhas.
    /// - O controller retorna respostas HTTP apropriadas para cada cenário (sucesso, erro de validação, não autorizado).
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //Injeção de dependência do serviço de autenticação
        private readonly AuthService _authService;

        //Construtor que recebe o serviço de autenticação
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        //Metodo para registrar um novo usuário
        [HttpPost]
        [Route("auth/register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDTO userDTO)
        {
            //Realiza a validação dos dados do usuário
            var result = await _authService.RegisterAsync(
                userDTO.FirstName,
                userDTO.LastName,
                userDTO.Email,
                userDTO.CPFPassport,
                userDTO.Password
            );
            //Verifica se o registro foi bem-sucedido
            if (result.Succeeded)
            {
                //Verifica se o usuário foi criado com sucesso
                //TODO: Enviar um email de confirmação de registro
                //Returna uma resposta de sucesso
                return Ok("User registered successfully");
            }
            //Se o registro falhar, retorna uma resposta de erro com as mensagens de validação
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        // Método para autenticar um usuário
        [HttpPost]
        [Route("auth/login")]
        public Task<IActionResult> Login([FromBody] LoginUserRequestDTO userDTO)
        {
            // Verifica se o email e a senha são válidos
            var result = _authService.LoginAsync(userDTO.Email, userDTO.Password);

            // Verifica se o login foi bem-sucedido
            if (result.Result.Succeeded)
            {
                // Autentica o usuário na aplicação
                return Task.FromResult<IActionResult>(Ok("Login successful"));
            }
            // Se o login falhar, retorna uma resposta de erro
            return Task.FromResult<IActionResult>(Unauthorized("Invalid email or password"));
        }

        // Método para gerar um link de redefinição de senha
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordDTO)
        {
            // Verifica se o email é válido
            var user = await _authService.FindByEmailAsync(forgotPasswordDTO.Email);
            // Verifica se o usuário existe
            if (user == null)
            {
                return NotFound("Email not found");
            }
            // Verifica se o usuário existe
            var result = await _authService.GeneratePasswordResetTokenAsync(user);

            //Codifica o Token para URL
            var encondedToken = System.Net.WebUtility.UrlEncode(result);

            // Gera o link de redefinição de senha
            var resetLink = Url.Action(
              "ResetPassword",
                "Auth",
                new { token = encondedToken, email = forgotPasswordDTO.Email },
                Request.Scheme
            );

            // Retorno de comfirmação de envio do email de redefinição de senha
            return Ok($"Password reset link sent to email address: {forgotPasswordDTO.Email}");

        }
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery]string token, [FromQuery]string email,[FromBody]NewPasswordRequestDTO newPassword)
        {
            // Verifica se o email é válido
            var user = await _authService.FindByEmailAsync(email);

            // Verifica se o usuário não este vazio ou nulo
            if (user == null)
            {
                return NotFound(new JSONResponseDTO<object>(
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: "User not found",
                        Success: false,
                        Data: DateTime.UtcNow.ToString("o")
                    ));
            }

            // Verifica se o token não está vazio ou nulo
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new JSONResponseDTO<object>(
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "Token is required",
                        Success: false,
                        Data: DateTime.UtcNow.ToString("o")
                    ));
            }

            // Verifica se a nova senha não está vazia ou nula
            if (string.IsNullOrEmpty(newPassword.NewPassword))
            {
                return BadRequest("New password is required");
            }

            // Decodifica o token
            var rawToken = System.Net.WebUtility.UrlDecode(token);

            // Verifica se o token é válido e redefine a senha
            var result = await _authService.ResetPasswordAsync(user, token, newPassword.NewPassword);
          
            //EmailConfiguration.SendPasswordResetEmail(email, "Your password has been reset successfully.");
           return Ok("Your password has been reset successfully.");
        }
    }
}
