using travel_agency_back.Data;
using travel_agency_back.Models;

namespace travel_agency_back.Repositories
{
    /// <summary>
    /// Repositório responsável por gerenciar a relação entre usuários e pacotes de viagem.
    /// 
    /// Esta classe contém métodos para:
    /// - Verificar se um usuário já possui uma reserva específica
    /// - Adicionar uma nova reserva (booking) para um usuário
    /// 
    /// É utilizada principalmente no processo de compra de pacotes,
    /// evitando duplicações e gerenciando o histórico de reservas.
    /// </summary>
    public class UserPackageRepository
    {
        private ApplicationDBContext _context; // Contexto do Entity Framework para acesso ao banco de dados

        /// <summary>
        /// Construtor que injeta o contexto do banco de dados
        /// </summary>
        /// <param name="context">Contexto do Entity Framework (ApplicationDBContext)</param>
        public UserPackageRepository(ApplicationDBContext context)
        {
            _context = context; // Injeta o contexto do banco de dados via dependency injection
        }

        /// <summary>
        /// Verifica se já existe uma reserva (booking) de um pacote específico para um usuário
        /// 
        /// Este método é usado para evitar que um usuário faça múltiplas reservas
        /// do mesmo pacote, garantindo integridade de negócio.
        /// </summary>
        /// <param name="userId">ID do usuário a ser verificado</param>
        /// <param name="packageId">ID do pacote a ser verificado</param>
        /// <returns>True se a reserva já existe, False caso contrário</returns>
        public bool UserPackageExists(int userId, int packageId)
        {
            // Busca no DbSet UserBookings por uma reserva que combine userId e packageId
            // FirstOrDefault retorna null se não encontrar nenhum registro
            var userPackage = _context.UserBookings
                .FirstOrDefault(up => up.UserId == userId && up.PackageId == packageId);
            
            // Retorna true se encontrou uma reserva, false se userPackage é null
            return userPackage != null;
        }

        /// <summary>
        /// Adiciona uma nova reserva (booking) de pacote para um usuário
        /// 
        /// Este método cria uma nova entrada na tabela UserBookings com todos os dados
        /// relacionados: usuário, pacote, acompanhantes e pagamentos.
        /// </summary>
        /// <param name="userId">ID do usuário que está fazendo a reserva</param>
        /// <param name="packageId">ID do pacote sendo reservado</param>
        /// <param name="companions">Lista de acompanhantes da viagem</param>
        /// <param name="payments">Lista de pagamentos associados à reserva</param>
        public void UserPackageAdd(int userId, int packageId, List<Companions> companions, List<Payments> payments)
        {
            // Cria uma nova instância de Booking (reserva) com os dados fornecidos
            var userPackage = new Booking
            {
                UserId = userId,                    // FK para a tabela Users
                PackageId = packageId,              // FK para a tabela Packages
                CreatedAt = DateTime.UtcNow,        // Data/hora atual de criação da reserva
                Companions = companions,            // Lista de acompanhantes (relacionamento 1:N)
                Payments = payments,                // Lista de pagamentos (relacionamento 1:N)
            };
            
            // Adiciona a nova reserva ao contexto UserBookings (ainda não persiste no banco)
            _context.UserBookings.Add(userPackage);
            
            // Persiste todas as mudanças no banco de dados
            // Isso inclui a reserva principal e todas as entidades relacionadas (companions, payments)
            _context.SaveChanges();
        }

    }
}
