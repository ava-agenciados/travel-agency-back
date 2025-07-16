using travel_agency_back.Data;
using travel_agency_back.Models;

namespace travel_agency_back.Repositories
{
    public class UserPackageRepository
    {
        private ApplicationDBContext _context;
        public UserPackageRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool UserPackageExists(int userId, int packageId)
        {
            // Verifica se existe um pacote associado ao usuário
            var userPackage = _context.UserBookings
                .FirstOrDefault(up => up.UserId == userId && up.PackageId == packageId);
            return userPackage != null;
        }

        public void UserPackageAdd(int userId, int packageId, List<Companions> companions, List<Payments> payments)
        {
            // Adiciona um pacote ao usuário
            var userPackage = new Booking
            {
                UserId = userId,
                PackageId = packageId,
                CreatedAt = DateTime.UtcNow,
                Companions = companions, // Inicializa a lista de acompanhantes
                Payments = payments, // Inicializa a lista de pagamentos
            };
            _context.UserBookings.Add(userPackage);
            _context.SaveChanges();
        }

    }
}
