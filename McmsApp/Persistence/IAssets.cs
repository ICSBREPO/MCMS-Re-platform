using McmsApp.Models;

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
