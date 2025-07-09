using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.Models;

namespace travel_agency_back.Data
{
    /// <summary>
    /// Representa o contexto principal do Entity Framework Core para a aplicação.
    /// 
    /// Herda de <see cref="IdentityDbContext{User}"/>, integrando o ASP.NET Core Identity ao banco de dados
    /// e permitindo o gerenciamento de autenticação, autorização e dados dos usuários personalizados.
    /// 
    /// Funcionalidades principais:
    /// - Gerencia o ciclo de vida das entidades relacionadas ao Identity, incluindo usuários, roles e claims.
    /// - Permite a configuração de DbSets adicionais para outras entidades do domínio, caso necessário.
    /// - Utiliza o modelo <see cref="User"/> personalizado, que inclui propriedades como FirstName, LastName e CPFPassport.
    /// 
    /// O construtor recebe as opções de configuração do contexto, normalmente injetadas pelo sistema de dependência do ASP.NET Core.
    /// </summary>
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        //Metodo construtor que recebe as opções de configuração do contexto
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
    }
}
