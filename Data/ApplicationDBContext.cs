using Microsoft.AspNetCore.Identity;
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
    public class ApplicationDBContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        //Metodo construtor que recebe as opções de configuração do contexto  
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        public DbSet<UserDocument> UserDocuments { get; set; } // DbSet para a entidade UserDocument
        public DbSet<Rating> Rating { get; set; } // DbSet para a entidade Rating
        public DbSet<Booking> Bookings { get; set; } // DbSet para a entidade Booking
        public DbSet<Companions> Companions { get; set; } // DbSet para a entidade Companions
        public DbSet<Payments> Payments { get; set; } // DbSet para a entidade Payments
        public DbSet<PaymentLogs> PaymentLogs { get; set; } // DbSet para a entidade PaymentLogs
        public DbSet<Booking> UserBookings { get; set; } // DbSet para a entidade UserBooking
        public DbSet<PackageMedia> PackageMedia { get; set; } // DbSet para a entidade PackageMedia
        public DbSet<Packages> Packages { get; set; } // DbSet para a entidade Packages  
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configurações adicionais do modelo podem ser feitas aqui, se necessário.  

            builder.Entity<User>(e =>
            {
                e.ToTable("Usuarios"); // Define o nome da tabela no banco de dados  
            });
            builder.Entity<IdentityRole<int>>(e =>
            {
                e.ToTable("Roles"); // Define o nome da tabela de roles no banco de dados  
            });
            builder.Entity<IdentityUserRole<int>>(e =>
            {
                e.ToTable("UsuarioRoles"); // Define o nome da tabela de associação entre usuários e roles  
            });
            builder.Entity<IdentityUserClaim<int>>(e =>
            {
                e.ToTable("UsuarioClaims"); // Define o nome da tabela de claims dos usuários  
            });
            builder.Entity<IdentityUserLogin<int>>(e =>
            {
                e.ToTable("UsuarioLogins"); // Define o nome da tabela de logins dos usuários  
            });
            builder.Entity<IdentityRoleClaim<int>>(e =>
            {
                e.ToTable("RoleClaims"); // Define o nome da tabela de claims das roles  
            });
            builder.Entity<IdentityUserToken<int>>(e =>
            {
                e.ToTable("UsuarioTokens"); // Define o nome da tabela de tokens dos usuários  
            });

            // User 1:1 UserDocument
            builder.Entity<User>()
                .HasOne(u => u.UserDocument)
                .WithOne(d => d.User)
                .HasForeignKey<UserDocument>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1:N Booking
            builder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1:N Rating
            builder.Entity<User>()
                .HasMany(u => u.Ratings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Packages 1:N Booking
            builder.Entity<Packages>()
                .HasMany(p => p.Bookings)
                .WithOne(b => b.Package)
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Packages 1:N Rating
            builder.Entity<Packages>()
                .HasMany(p => p.Ratings)
                .WithOne(r => r.Package)
                .HasForeignKey(r => r.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Packages 1:N PackageMedia
            builder.Entity<Packages>()
                .HasMany(p => p.PackageMedia)
                .WithOne(m => m.Package)
                .HasForeignKey(m => m.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking 1:N Companions
            builder.Entity<Booking>()
                .HasMany(b => b.Companions)
                .WithOne(c => c.Booking)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking 1:N Payments
            builder.Entity<Booking>()
                .HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payments 1:N PaymentLogs
            builder.Entity<Payments>()
                .HasMany(p => p.PaymentLogs)
                .WithOne(l => l.Payment)
                .HasForeignKey(l => l.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
