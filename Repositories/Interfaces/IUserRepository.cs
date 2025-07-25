using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;

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

        public Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
        public Task<User> GetUserByIdAsync(int userId);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> GetUserByCPFPassportAsync(string CPFPassport);

        //Pacotes do usuario

    }
}
