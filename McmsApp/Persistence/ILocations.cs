using McmsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
