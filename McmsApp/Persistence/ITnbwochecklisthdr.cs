using mcms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
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
