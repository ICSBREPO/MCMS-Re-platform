using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ILocations
    {
        Task<List<Locations>> GetLocationsByWonum(string wonum);
        Task<Asset> GetLocations(int id);
        Task AddLocations(Locations d);
        Task DeleteLocations(Locations d);
        Task UpdateLocations(Locations d);
        Task ResetLocations();
        Task<int?> CountLocations(string wonum);
    }
}
