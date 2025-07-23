using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests.Media;

namespace travel_agency_back.Services.Interfaces
{
    public interface IMediaService
    {
        public Task<IActionResult> GetPackageMediaAsync(int packageId);
        public Task<IActionResult> GetMediaByIdAsync(int mediaId);
        public Task<IActionResult> UploadMediaAsync(int packageId, List<IFormFile> medias);
        public Task<IActionResult> DeleteMediaAsync(int mediaId);
        public Task<IActionResult> UpdateMediaAsync(int mediaId, IFormFile file);
        public Task<IActionResult> GetAllMediaAsync(int packageId);
    }
}
