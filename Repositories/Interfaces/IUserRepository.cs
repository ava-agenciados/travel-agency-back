namespace travel_agency_back.Repositories.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Funções responsaveis por manipular os dados do usuário
        /// CreateUser: Cria um novo usuário
        /// UserEmailExists: Verifica se o email já está cadastrado
        /// UserCpfPassportExists: Verifica se o CPF ou passaporte já está cadastrado
        /// 

        public void CreateUser(string firstName, string lastName, string cpfPassport, string email, string password);

        public bool UserEmailExists(string email);

        public bool UserCpfPassportExists(string cpfPassport);
        
    }
}
