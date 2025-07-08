namespace travel_agency_back.Services.Interfaces
{
    public interface IUserService
    {
        public void CreateUser(string firstName, string lastName, string cpfPassport, string email, string password);
        public bool UserEmailExists(string email);
        public bool UserCpfPassportExists(string cpfPassport);
    }
}
