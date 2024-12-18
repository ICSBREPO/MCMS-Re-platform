using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IWorklog
    {
        Task<List<Worklog>> GetWorklogByWonum(string wonum);
        Task<Worklog> GetWorklog(int id);
        Task AddWorklog(Worklog d);
        Task DeleteWorklog(Worklog d);
        Task UpdateWorklog(Worklog d);
        Task ResetWorklog();
        Task<int?> CountWorklog(string wonum);
    }
}
