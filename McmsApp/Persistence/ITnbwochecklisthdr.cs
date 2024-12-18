using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ITnbwochecklisthdr
    {
        Task<List<Tnbwochecklisthdr>> GetTnbwochecklisthdrAsync();
        Task<Tnbwochecklisthdr> GetTnbwochecklisthdr(int id);
        Task<Tnbwochecklisthdr> GetTnbwochecklisthdrs(string tnbwonum);
        Task<int?> AddTnbwochecklisthdr(Tnbwochecklisthdr d);
        Task DeleteTnbwochecklisthdr(Tnbwochecklisthdr d);
        Task ResetTnbwochecklisthdr();
        Task UpdateTnbwochecklisthdr(Tnbwochecklisthdr d);
        Task<List<Tnbwochecklisthdr>> GetTnbwochecklisthdrByGroup(int tnbpsiwochecklisttypeid);
    }
}
