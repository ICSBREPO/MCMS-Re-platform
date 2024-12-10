using McmsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
    public interface IAsset
    {
        Task<List<Asset>> GetAssetByWonum(string wonum);
        Task<Asset> GetAsset(int id);
        Task AddAsset(Asset d);
        Task DeleteAsset(Asset d);
        Task UpdateAsset(Asset d);
        Task ResetAsset();
        Task<int?> CountAsset(string wonum);
    }
}
