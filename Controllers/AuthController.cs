using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using travel_agency_back.DTOs.Requests;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.Services;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;
using Swashbuckle.AspNetCore.Annotations;

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
        private readonly IAuthService _authService;

        //Construtor que recebe o serviço de autenticação
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra um novo usuário na aplicação.
        /// </summary>
        /// <remarks>Endpoint público para cadastro de clientes.</remarks>
        [SwaggerOperation(
            Summary = "Registra um novo usuário na aplicação",
            Description = "Endpoint público para cadastro de clientes."
        )]
        //Metodo para registrar um novo usuário
        [HttpPost]
        [Route("auth/register")]
        public async Task<IActionResult> Register([FromBody] CreateUserRequestDTO userDTO)
        {
            //Realiza a validação dos dados do usuário
            var UserRegister = await _authService.RegisterAsync(
                userDTO.FirstName,
                userDTO.LastName,
                userDTO.Email,
                userDTO.PhoneNumber,
                userDTO.CPFPassport,
                userDTO.Password
            );
            return Ok(UserRegister);
        }

        /// <summary>
        /// Realiza login e retorna o token JWT.
        /// </summary>
        /// <remarks>Endpoint público para autenticação de clientes.</remarks>
        [SwaggerOperation(
            Summary = "Realiza login e retorna o token JWT",
            Description = "Endpoint público para autenticação de clientes."
        )]
        // Novo método de login que retorna o token JWT
        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDTO userDTO)
        {
            if (userDTO == null || string.IsNullOrEmpty(userDTO.Email) || string.IsNullOrEmpty(userDTO.Password))
            {
                return BadRequest(new GenericResponseDTO(400, "Endereço de e-mail e senha são obrigatórios", false));
            }
            // Tenta autenticar o usuário e gerar o token JWT
            var (result, token) = await _authService.LoginWithTokenAsync(userDTO.Email, userDTO.Password);

            // Verifica se a autenticação foi bem-sucedida
            if (result.Succeeded && !string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append(
                    "jwt",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Use true em produção (HTTPS)
                        SameSite = SameSiteMode.Strict, // Ou Lax, conforme necessidade
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    }
                );
                return Ok(new GenericResponseDTO(200, "Usuário autenticado com sucesso!", true));
            }
            return Unauthorized(new GenericResponseDTO(401, "Endereço de e-mail ou senha estão incorretos", false));
        }

        /// <summary>
        /// Realiza logout do usuário autenticado.
        /// </summary>
        /// <remarks>Endpoint protegido, remove o cookie JWT.</remarks>
        [SwaggerOperation(
            Summary = "Realiza logout do usuário autenticado",
            Description = "Endpoint protegido, remove o cookie JWT."
        )]
        [HttpPost]
        [Route("auth/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie JWT ao fazer logout
            //TODO: REFATORAR ISSO AQUI PELO AMOR!!!!!!!!!!!!!!!!!!!!
            Response.Cookies.Delete("jwt");
            return Ok(new GenericResponseDTO(200, "Usuário deslogado com sucesso!", true));
        }

        /// <summary>
        /// Inicia o processo de recuperação de senha.
        /// </summary>
        /// <remarks>Endpoint público, envia instruções para o e-mail informado.</remarks>
        [SwaggerOperation(
            Summary = "Inicia o processo de recuperação de senha",
            Description = "Endpoint público, envia instruções para o e-mail informado."
        )]
        [HttpPost]
        [Route("auth/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new GenericResponseDTO(400, "Endereço de e-mail é obrigatório", false));
            }

            var (result, url, emailSent) = await _authService.GeneratePasswordResetTokenAsync(request.Email);
            if (!result.Succeeded)
            {
                return BadRequest(new GenericResponseDTO(400, "Erro ao gerar token de redefinição de senha", false));
            }
            return Ok(new GenericResponseDTO(200, $"Instruções para redefinir a senha foram enviadas para o seu e-mail {request.Email}", true));
        }

        /// <summary>
        /// Redefine a senha do usuário.
        /// </summary>
        /// <remarks>Endpoint público, redefine a senha usando token enviado por e-mail.</remarks>
        [SwaggerOperation(
            Summary = "Redefine a senha do usuário",
            Description = "Endpoint público, redefine a senha usando token enviado por e-mail."
        )]
        [HttpPatch]
        [Route("auth/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            var resetResult = await _authService.ResetPasswordAsync(request.Token, request.Email, request.Password);
            return Ok(resetResult);
        }   
    }
}
