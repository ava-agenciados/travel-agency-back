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
        public void AddPackageMedia(int packageID, string mediaUrl)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            var packageMedia = new PackageMedia
            {
                PackageId = packageID,
                ImageURL = mediaUrl,
                CreatedAt = DateTime.UtcNow
            };
            _context.PackageMedias.Add(packageMedia);
        }

        public void AddRatingToPackage(int packageID, Rating rating)
        {
            
            
        }

        public void CreatePackage(Packages package)
        {
            var existingPackage = _context.Packages.FirstOrDefault(p => p.Name == package.Name && p.Destination == package.Destination);
            if (existingPackage != null)
            {
                throw new ArgumentException("Package with the same name and destination already exists.");
            }
            package.CreatedAt = DateTime.UtcNow;
            _context.Packages.Add(package);
        }

        public void DeletePackage(int packageID)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            _context.Packages.Remove(package);
        }

        public void DeleteRating(int ratingID)
        {
            var rating = _context.Ratings.Find(ratingID);
            if (rating == null)
            {
                throw new ArgumentException("Rating not found");
            }
            _context.Ratings.Remove(rating);
        }

        public async Task<IEnumerable<Packages>> GetAllAvailablePackagesAsync()
        {
            var packages = await _context.Packages
                .Where(p => p.IsAvailable)
                .ToListAsync();
            return packages;
        }

        public List<Packages> GetAllPackages()
        {
            var packages = _context.Packages.ToList();
            return packages;
        }

        public List<Rating> GetCommentsByPackageId(int packageID)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            var ratings = _context.Ratings.Where(r => r.PackageId == packageID).ToList();
            return ratings;
        }

        public List<Rating> GetCommentsFromAllPackages()
        {
            var ratings = _context.Ratings.ToList();
            return ratings;
        }

        public Packages GetPackageById(int packageID)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            return package;
        }

        public List<Packages> GetPackagesByActiveDate(DateTime activeFrom, DateTime activeUntil)
        {
            var packages = _context.Packages
                .Where(p => p.ActiveFrom >= activeFrom && p.ActiveUntil <= activeUntil)
                .ToList();
            return packages;
        }

        public List<Packages> GetPackagesByBeginDate(DateTime beginDate)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByCreatedAt(DateTime createdAt)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByDescription(string description)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByDestination(string destination)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByEndDate(DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByIsAvailable(bool isAvailable)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByOrigin(string origin)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var packages = _context.Packages
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToList();
            return packages;
        }

        public List<Packages> GetPackagesByQuantity(int quantity)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByRating(int rating)
        {
            throw new NotImplementedException();
        }

        public List<Packages> GetPackagesByRatingRange(int minRating, int maxRating)
        {
            throw new NotImplementedException();
        }

        public void RemovePackageMedia(int packageID, string mediaUrl)
        {
            var packageMedia = _context.PackageMedias
                .FirstOrDefault(pm => pm.PackageId == packageID && pm.ImageURL == mediaUrl);
        }

        public void SetPackageAvailability(int packageID, bool isAvailable)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            package.IsAvailable = isAvailable;
        }

        public void UpdatePackage(int packageID, Packages packages)
        {
            var package = _context.Packages.Find(packageID);
            if (package == null)
            {
                throw new ArgumentException("Package not found");
            }
            package.Name = packages.Name;
            package.Description = packages.Description;
            package.Price = packages.Price;
            package.ImageUrl = packages.ImageUrl;
            package.ActiveFrom = packages.ActiveFrom;
            package.ActiveUntil = packages.ActiveUntil;
            package.BeginDate = packages.BeginDate;
            package.EndDate = packages.EndDate;
            package.Origin = packages.Origin;
            package.Destination = packages.Destination;
            package.Quantity = packages.Quantity;
            package.IsAvailable = packages.IsAvailable;
            package.CreatedAt = packages.CreatedAt;
            _context.Packages.Update(package);
        }

        public void UpdatePackage(int packageID)
        {
            throw new NotImplementedException();
        }

        public void UpdateRating(int ratingID, Rating updatedRating)
        {
            throw new NotImplementedException();
        }
    }
}
