using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IChecklistVal
    {
        Task<List<Tnbchecklistvalue>> GetValueAsync(string checklist);
        Task<Tnbchecklistvalue> GetValue(int id);
        Task AddValue(Tnbchecklistvalue d);
        Task UpdateValue(Tnbchecklistvalue d);
        Task DeleteValue(Tnbchecklistvalue d);
        Task ResetValue();
    }
}
