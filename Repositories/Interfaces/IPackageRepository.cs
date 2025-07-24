using Microsoft.AspNetCore.Mvc;
using travel_agency_back.Models;

namespace travel_agency_back.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        //Funções CRUD para Pacotes
        public Task<IActionResult> CreateNewPackageAsync(Packages package); // Cria um novo pacote
        public Task<IActionResult> DeletePackageByIdAsync(int packageID);// Deleta um pacote pelo ID
        public Task<IActionResult> UpdatePackageByIdAsync(int packageID, Packages package); // Atualiza um pacote pelo ID

        public Task<IActionResult> SaveChangesAsync();

        //Funções de consulta
        public Task<Packages> GetPackageByIdAsync(int packageID); // Retorna um pacote específico pelo ID
        public Task<IEnumerable<Packages>> GetAllPackagesAsync(); // Retorna todos os pacotes

        public Task<List<Packages>> GetPackagesByFilter(string? origin, string? destination, DateTime? startDate, DateTime? endDate);

        public Task<IActionResult> AddComment(Rating rating);

        public Task<IActionResult> DeleteRating(int ratingId);

        public Task<List<Packages>> GetMostLovedPackages();


        public Task<List<string>> GetAllDestinations();




    }
}
