using Microsoft.AspNetCore.Mvc;
using travel_agency_back.DTOs.Requests.Media;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;

namespace travel_agency_back.Services
{
    public class MediaService : IMediaService
    {

        private readonly IPackageRepository _packageRepository;

        public MediaService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }
        public Task<IActionResult> DeleteMediaAsync(int mediaId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetAllMediaAsync(int packageId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetMediaByIdAsync(int mediaId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateMediaAsync(int mediaId, IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> UploadMediaAsync(int packageId, List<IFormFile> medias)
        {
            if(medias.Count == 0 || !medias.Any())
            {
                return await Task.FromResult<IActionResult>(new BadRequestObjectResult(new { message = "Nenhum arquivo enviado." }));
            }
            foreach(var media in medias)
            {
                if(media.Length == 0)
                {
                    return await Task.FromResult<IActionResult>(new BadRequestObjectResult(new { message = "Arquivo vazio." }));
                }
                //Verifica se o tipo de arquivo é suportado(foto ou video)
                if (media.ContentType.StartsWith("image/") || media.ContentType.StartsWith("video/"))
                {
                    var filePath = Path.Combine("wwwroot", "uploads", packageId.ToString(), media.FileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        media.CopyTo(stream);
                    }
                }
                //Verifica o tipo de mídia e salva no banco de dados
                if (!media.ContentType.StartsWith("image/") && !media.ContentType.StartsWith("video/"))
                {
                    return await Task.FromResult<IActionResult>(new BadRequestObjectResult(new { message = "Formato de mídia não autorizado." }));
                }

                var package = await _packageRepository.GetPackageByIdAsync(packageId);
                if (package == null)
                {
                    return await Task.FromResult<IActionResult>(new NotFoundObjectResult(new { message = "Pacote não encontrado." }));
                }
                // Cria um novo objeto PackageMedia para armazenar a imagem
                var picture = new PackageMedia
                {
                    CreatedAt = DateTime.UtcNow,
                    PackageId = packageId,
                    MediaType = 1,
                    ImageURL = Path.Combine("uploads", packageId.ToString(), media.FileName)
                };
                package.PackageMedia.Add(picture);
                await _packageRepository.UpdatePackageByIdAsync(packageId, package);
            }
            return await Task.FromResult<IActionResult>(new OkObjectResult(new { message = "Mídia enviada com sucesso." }));
        }

        public async Task<IActionResult> DeleteMediaFromPackageAsync(int packageId, string mediaName)
        {
            var package = await _packageRepository.GetPackageByIdAsync(packageId);
            if (package == null)
                return new NotFoundObjectResult(new { message = "Pacote não encontrado." });
            var media = package.PackageMedia?.FirstOrDefault(m => m.ImageURL.EndsWith(mediaName));
            if (media == null)
                return new NotFoundObjectResult(new { message = "Mídia não encontrada." });
            package.PackageMedia.Remove(media);
            await _packageRepository.UpdatePackageByIdAsync(packageId, package);
            // Remove o arquivo físico se existir
            var filePath = Path.Combine("wwwroot", media.ImageURL);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            return new OkObjectResult(new { message = "Mídia deletada com sucesso!" });
        }

        public async Task<IActionResult> GetPackageMediaAsync(int packageId)
        {
            var package = await _packageRepository.GetPackageByIdAsync(packageId);
            if (package == null)
                return new NotFoundObjectResult(new { message = "Pacote não encontrado." });
            var mediaList = package.PackageMedia?.Select(pm => new DTOs.Resposes.Packages.PackageMediaResponseDTO
            {
                Id = pm.Id,
                MediaType = pm.MediaType,
                MediaUrl = pm.ImageURL
            }).ToList() ?? new List<DTOs.Resposes.Packages.PackageMediaResponseDTO>();
            return new OkObjectResult(mediaList);
        }
    }
}
