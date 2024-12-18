using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IWorkorder
    {
        Task<List<Workorder>> GetWorkorderAsync();
        Task<List<WOChart>> GetWOChart(string attr);
        Task<List<Workorder>> GetWorkorderInprg();
        Task<List<Workorder>> GetWorkorderAppr();
        Task<List<Workorder>> GetSearchWorkorder(string search);
        Task<List<Workorder>> GetFilterWorkorder(string where);
        Task<List<Distinct>> GetListDistinct(string attr);
        Task<Workorder> GetWorkorder(int id);
        Task<Workorder> GetParentWorkorder(string parent);
        Task<int> AddWorkorder(Workorder d);
        Task DeleteWorkorder(Workorder d);
        Task UpdateWorkorder(Workorder d);
        Task ResetWorkorder();
        Task<List<Workorder>> GetChildWorkorders(string wonum);
        Task<int> CountWorkorder();
        Task<int?> CountChild(string wonum);
    }
}
