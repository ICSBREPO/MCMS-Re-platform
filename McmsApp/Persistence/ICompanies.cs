using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ICompanies
    {
        Task<List<Companies>> GetCompaniesAsync();
        Task<Companies> GetCompanies(int Id);
        Task AddCompanies(Companies d);
        Task UpdateCompanies(Companies d);
        Task DeleteCompanies(Companies d);
        Task ResetCompanies();
    }
}
