using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ITnbmetergroup
    {
        Task<List<Metergroup>> GetTnbmetergroupAsync();
        Task<Metergroup> GetTnbmetergroup(int id);
        Task AddTnbmetergroup(Metergroup d);
        Task DeleteTnbmetergroup(Metergroup d);
        Task UpdateTnbmetergroup(Metergroup d);
        Task ResetTnbmetergroup();
    }
}
