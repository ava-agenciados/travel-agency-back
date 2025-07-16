using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }
        public Task<IEnumerable<Packages>> GetAllAvailablePackagesAsync()
        {
            return _packageRepository.GetAllAvailablePackagesAsync();
        }
    }
}
