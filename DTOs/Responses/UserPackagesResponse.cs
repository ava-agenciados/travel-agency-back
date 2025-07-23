using travel_agency_back.DTOs.Resposes.Packages;

namespace travel_agency_back.DTOs.Resposes
{
    public class UserPackagesResponse
    {
       public IEnumerable<PackageResponseDTO> packages { get; set; } = new List<PackageResponseDTO>();

    }
}
