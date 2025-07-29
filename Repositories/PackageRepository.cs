using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;

namespace travel_agency_back.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private ApplicationDBContext _context;
        public PackageRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AddComment(Rating rating)
        {
            var result = await _context.Rating.AddAsync(rating);
            if (result == null)
            {
                return new BadRequestObjectResult(new { Message = "Erro ao adicionar comentário" });
            }
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Message = "Comentário adicionado com sucesso", Rating = result.Entity });

        }

        public async Task<IActionResult> CreateNewPackageAsync(Packages package)
        {
            // Se LodgingInfo existir, salva primeiro para garantir o Id autoincrementado
            if (package.LodgingInfo != null)
            {
                _context.LodgingInfos.Add(package.LodgingInfo);
                await _context.SaveChangesAsync();
                package.LodgingInfoId = package.LodgingInfo.Id;
            }

            var newPackage = new Packages
            {
                Name = package.Name,
                Description = package.Description,
                Origin = package.Origin,
                Destination = package.Destination,
                Price = package.Price,
                ActiveFrom = package.ActiveFrom,
                ActiveUntil = package.ActiveUntil,
                BeginDate = package.BeginDate,
                EndDate = package.EndDate,
                Quantity = package.Quantity,
                IsAvailable = package.IsAvailable,
                ImageUrl = package.ImageUrl,
                Bookings = new List<Booking>(),
                Ratings = new List<Rating>(),
                PackageMedia = package.PackageMedia?.Select(pm => new PackageMedia { ImageURL = pm.ImageURL, MediaType = pm.MediaType }).ToList(),
                CreatedAt = DateTime.UtcNow,
                LodgingInfoId = package.LodgingInfoId,
                DiscountPercent = package.DiscountPercent
            };
            _context.Packages.Add(newPackage);
            await _context.SaveChangesAsync();
            return new OkObjectResult(newPackage);
        }

        public async Task<IActionResult> DeletePackageByIdAsync(int packageID)
        {
            var package = _context.Packages.Include(p => p.LodgingInfo).FirstOrDefault(p => p.Id == packageID);
            if (package == null)
            {
                return await Task.FromResult<IActionResult>(new NotFoundObjectResult(new { Message = "Pacote não encontrado" }));
            }
            if (package.LodgingInfo != null)
            {
                _context.LodgingInfos.Remove(package.LodgingInfo);
            }
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            return await Task.FromResult<IActionResult>(new OkObjectResult(new { Message = "Pacote deletado com sucesso" }));
        }

        public async Task<List<string>> GetAllDestinations()
        {
            return await _context.Packages
                .Select(p => p.Destination)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Packages>> GetAllPackagesAsync()
        {
            var packages = await _context.Packages
                .Include(p => p.Bookings)
                .Include(p => p.Ratings).ThenInclude(r => r.User)
                .Include(p => p.PackageMedia)
                .Include(p => p.LodgingInfo) // Inclui LodgingInfo
                .ToListAsync(); // Removido o filtro .Where(p => p.IsAvailable)
            return packages;
        }

        public async Task<List<Packages>> GetMostLovedPackages()
        {
            return await _context.Packages
                .Include(p => p.Ratings)
                .Include(p => p.PackageMedia)
                .Include(p => p.Bookings)
                .Include(p => p.LodgingInfo) // Inclui LodgingInfo
                .Where(p => p.Ratings.Any()) // Apenas pacotes com avaliações
                .OrderByDescending(p => p.Ratings.Average(r => r.Stars))
                .ThenByDescending(p => p.Ratings.Count)
                .Take(10) // Retorna os 10 mais amados
                .ToListAsync();
        }

        public async Task<Packages> GetPackageByIdAsync(int packageID)
        {
            var package = await _context.Packages
                .Include(p => p.Bookings)
                .Include(p => p.Ratings)
                .Include(p => p.PackageMedia)
                .Include(p => p.LodgingInfo) // Inclui LodgingInfo
                .FirstOrDefaultAsync(p => p.Id == packageID); // Removido o filtro .Where(p => p.IsAvailable)
            if (package == null)
            {
                return null;
            }
            return package;
        }

        public async Task<List<Packages>> GetPackagesByFilter(string? origin, string? destination, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Packages
                .Include(p => p.Bookings)
                .Include(p => p.Ratings)
                .Include(p => p.PackageMedia)
                .Include(p => p.LodgingInfo) // Inclui LodgingInfo
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrEmpty(origin))
                query = query.Where(p => p.Origin == origin);

            if (!string.IsNullOrEmpty(destination))
                query = query.Where(p => p.Destination == destination);

            if (startDate.HasValue)
                query = query.Where(p => p.BeginDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.EndDate <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<IActionResult> SaveChangesAsync()
        {
            // Apenas salva as alterações já rastreadas e marcadas como modificadas
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new OkObjectResult(new { Message = "Alterações salvas com sucesso." });
            }
            else
            {
                return new BadRequestObjectResult(new { Message = "Nenhuma alteração foi realizada." });
            }
        }

        public async Task<IActionResult> UpdatePackageByIdAsync(int packageID, Packages package)
        {
            // Busca o pacote com rastreamento e inclui LodgingInfo
            var existingPackage = await _context.Packages
                .Include(p => p.LodgingInfo)
                .FirstOrDefaultAsync(p => p.Id == packageID);
            if (existingPackage == null)
            {
                return new NotFoundObjectResult(new { Message = "Pacote não encontrado" });
            }
            // Atualiza os campos já preparados pelo AdminService
            existingPackage.Name = package.Name;
            existingPackage.Description = package.Description;
            existingPackage.Origin = package.Origin;
            existingPackage.Destination = package.Destination;
            existingPackage.Price = package.Price;
            existingPackage.ActiveFrom = package.ActiveFrom;
            existingPackage.ActiveUntil = package.ActiveUntil;
            existingPackage.BeginDate = package.BeginDate;
            existingPackage.EndDate = package.EndDate;
            existingPackage.Quantity = package.Quantity;
            existingPackage.IsAvailable = package.IsAvailable;
            existingPackage.ImageUrl = package.ImageUrl;
            if (package.PackageMedia != null && package.PackageMedia.Any())
                existingPackage.PackageMedia = package.PackageMedia.Select(pm => new PackageMedia { ImageURL = pm.ImageURL, MediaType = pm.MediaType }).ToList();
            // NÃO atualiza LodgingInfo aqui, pois já foi atualizado campo a campo no Service
            _context.Packages.Update(existingPackage);
            await _context.SaveChangesAsync();
            return new OkObjectResult(existingPackage);
        }
        public async Task<IActionResult> DeleteRating(int ratingId)
        {
            var rating = await _context.Rating.FindAsync(ratingId);
            if (rating == null)
            {
                return new NotFoundObjectResult(new { Message = "Avaliação não encontrada" });
            }
            _context.Rating.Remove(rating);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Message = "Avaliação deletada com sucesso" });
        }

        public void MarkLodgingInfoAsModified(LodgingInfo lodgingInfo)
        {
            if (_context.Entry(lodgingInfo).State == EntityState.Detached)
            {
                _context.LodgingInfos.Attach(lodgingInfo);
            }
            _context.Entry(lodgingInfo).State = EntityState.Modified;
        }
    }
}
