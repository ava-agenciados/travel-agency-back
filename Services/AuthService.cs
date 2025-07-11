using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; // Para acessar configurações
using Microsoft.IdentityModel.Tokens; // Para JWT
using System.IdentityModel.Tokens.Jwt; // Para JWT
using System.Security.Claims; // Para Claims
using System.Text; // Para Encoding
using travel_agency_back.Models;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    ///<summary>
    /// Serviço de autenticação responsável por login, registro e geração de tokens JWT.
    ///</summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration; // Para acessar configs do JWT

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<(SignInResult Result, string Token)> LoginWithTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (SignInResult.Failed, null);
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return (result, null);
            }
            // Gera o token JWT se login for bem-sucedido
            var token = GenerateJwtToken(user);
            return (result, token);
        }

        // Método para gerar o token JWT
        private string GenerateJwtToken(User user)
        {
            // Define as claims do token (informações do usuário)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            // Obtém configurações do appsettings.json
            var key = _configuration["Jwt:Key"] ?? "sua-chave-super-secreta";
            var issuer = _configuration["Jwt:Issuer"] ?? "travel-agency-api";
            var audience = _configuration["Jwt:Audience"] ?? "travel-agency-client";
            var expires = DateTime.UtcNow.AddHours(2); // Token válido por 2 horas

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<SignInResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterAsync(string firstname, string lastname, string email, string CPFPassport, string password)
        {
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                UserName = email,
                CPFPassport = CPFPassport
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return result;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<SignInResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            if (user == null || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return Task.FromResult(SignInResult.Failed);
            }
            var result = _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Result.Succeeded)
            {
                foreach (var error in result.Result.Errors)
                {
                    Console.WriteLine($"[Reset Error] {error.Code} - {error.Description}");
                }
            }
            return Task.FromResult(SignInResult.Success);
        }
    }
}
