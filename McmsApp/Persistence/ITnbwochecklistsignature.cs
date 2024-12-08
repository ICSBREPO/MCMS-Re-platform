using mcms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public interface ITnbwochecklistsignature
    {
        Task<List<Tnbwochecklistsignature>> GetTnbwochecklistsignatureAsync();
        Task<Tnbwochecklistsignature> GetTnbwochecklistsignature(int id);
        Task<int?> AddTnbwochecklistsignature(Tnbwochecklistsignature d);
        Task DeleteTnbwochecklistsignature(Tnbwochecklistsignature d);
        Task UpdateTnbwochecklistsignature(Tnbwochecklistsignature d);
        Task ResetTnbwochecklistsignature();
        Task<List<Tnbwochecklistsignature>> GetTnbwochecklistsignatureByGroup(int id);
      
    }
}
