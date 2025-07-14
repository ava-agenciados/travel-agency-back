using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace travel_agency_back.Utils
{
    public static class JWTGenerationToken
    {

        private static string? _KEY = "Jwt:Key"; // Define a chave secreta usada para assinar o token JWT
        private static string? _ISSUER = "Jwt:Issuer"; // Define o emissor do token JWT
        private static string? _AUDIENCE = "Jwt:Audience"; // Define o emissor e o público do token JWT
        private static DateTime _EXPIRATION_HOURS = DateTime.UtcNow.AddHours(24); // Define a expiração do token em horas(24 horas)

        // Método para obter a chave simétrica usada para assinar o token JWT
        public static string GenerateToken(string userId, string email, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            var key = configuration[_KEY];
            var issuer = configuration[_ISSUER];
            var audience = configuration[_AUDIENCE];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration is not properly set in appsettings.json");
            }

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: _EXPIRATION_HOURS,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
