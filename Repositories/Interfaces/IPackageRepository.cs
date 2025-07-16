using travel_agency_back.Models;

namespace travel_agency_back.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        public void CreatePackage(Packages package);
        public void UpdatePackage(int packageID);

        public void DeletePackage(int packageID);
        public Packages GetPackageById(int packageID);

        public List<Packages> GetAllPackages();
        public List<Packages> GetPackagesByDestination(string destination);
        public List<Packages> GetPackagesByPriceRange(decimal minPrice, decimal maxPrice);

        public List<Packages> GetPackagesByActiveDate(DateTime activeFrom, DateTime activeUntil);
        public List<Packages> GetPackagesByBeginDate(DateTime beginDate);
        public List<Packages> GetPackagesByEndDate(DateTime endDate); 
        public List<Packages> GetPackagesByOrigin(string origin);
        public List<Packages> GetPackagesByName(string name);
        public List<Packages> GetPackagesByDescription(string description);
        public List<Packages> GetPackagesByQuantity(int quantity);
        public List<Packages> GetPackagesByIsAvailable(bool isAvailable);
        public List<Packages> GetPackagesByCreatedAt(DateTime createdAt);

        public void SetPackageAvailability(int packageID, bool isAvailable);
        public void AddPackageMedia(int packageID, string mediaUrl);
        public void RemovePackageMedia(int packageID, string mediaUrl);
        public List<Packages> GetPackagesByRating(int rating);
        public List<Packages> GetPackagesByRatingRange(int minRating, int maxRating);

        public List<Rating> GetCommentsByPackageId(int packageID);
        public List<Rating> GetCommentsFromAllPackages();

        //Funções cliente
        public void AddRatingToPackage(int packageID, Rating rating);
        public void UpdateRating(int ratingID, Rating updatedRating);
        public void DeleteRating(int ratingID);
        Task<IEnumerable<Packages>> GetAllAvailablePackagesAsync();
    }
}
