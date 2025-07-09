using travel_agency_back.Data;
using travel_agency_back.Repositories.Interfaces;

namespace travel_agency_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext? _context;
        public UserRepository(ApplicationDBContext context)
        {
            this._context = context;
        }
    }
}
