using System.ComponentModel.DataAnnotations;

namespace travel_agency_back.DTOs.Requests.Packages
{
    public class UpdatePackageNameDTO
    {
        [Required]
        public int PackageID { get; set; }
        [Required]
        public int PackageName { get; set; }
    }
}
