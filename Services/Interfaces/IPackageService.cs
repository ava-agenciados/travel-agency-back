using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;

namespace travel_agency_back.Services.Interfaces
{
    public interface IPackageService
    {
        public Task<IEnumerable<Packages>> GetAllAvailablePackagesAsync();
    }
}
