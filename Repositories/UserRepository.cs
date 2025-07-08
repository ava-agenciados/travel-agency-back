using travel_agency_back.Repositories.Interfaces;

namespace travel_agency_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void CreateUser(string firstName, string lastName, string cpfPassport, string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool UserCpfPassportExists(string cpfPassport)
        {
            throw new NotImplementedException();
        }

        public bool UserEmailExists(string email)
        {
            throw new NotImplementedException();
        }
    }
}
