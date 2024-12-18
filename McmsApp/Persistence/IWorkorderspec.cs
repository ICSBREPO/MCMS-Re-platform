using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IWorkorderspec
    {
        Task<List<Workorderspec>> GetWorkorderspecAsync();
        Task<Workorderspec> GetWorkorderspec(int id);
        Task AddWorkorderspec(Workorderspec d);
        Task DeleteWorkorderspec(Workorderspec d);
        Task UpdateWorkorderspec(Workorderspec d);
        Task ResetWorkorderspec();
        Task<List<Workorderspec>> GetWorkorderspecByWO(int id);
        Task<int?> CountWOSpec(int id);
    }
}
