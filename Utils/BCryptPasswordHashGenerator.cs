using Microsoft.AspNetCore.Identity;
using BCrypt.Net;
namespace travel_agency_back.Utils
{
    /// <summary>
    /// Implementa o IPasswordHasher para integração com ASP.NET Core Identity,
    /// utilizando o algoritmo BCrypt para geração e verificação de hashes de senha.
    ///
    /// - Garante que as senhas dos usuários sejam armazenadas de forma segura.
    /// - O método HashPassword gera um hash seguro para a senha fornecida.
    /// - O método VerifyHashedPassword compara uma senha em texto puro com o hash armazenado,
    ///   retornando o resultado da verificação.
    /// - Compatível com qualquer classe de usuário utilizada pelo Identity.
    /// 
    /// Recomenda-se utilizar esta implementação quando se deseja usar BCrypt em vez do padrão PBKDF2 do Identity.
    /// </summary>
    public class BcryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public string HashPassword(TUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword) 
                ? PasswordVerificationResult.Success 
                : PasswordVerificationResult.Failed;
        }
    }
}
