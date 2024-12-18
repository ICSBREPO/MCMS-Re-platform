using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IAssetSpec
    {
        Task<List<AssetSpec>> GetAssetSpecAsync();
        Task<AssetSpec> GetAssetSpec(int id);
        Task AddAssetSpec(AssetSpec d);
        Task DeleteAssetSpec(AssetSpec d);
        Task UpdateAssetSpec(AssetSpec d);
        Task ResetAssetSpec();
        Task<List<AssetSpec>> GetAssetSpecByWO(int id);
        Task<int?> CountAssetSpec(int id);
    }
}
