using McmsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
    public interface ITnbwochecklisttype
    {
        Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeAsync();
        Task<Tnbwochecklisttype> GetTnbwochecklisttype(int id);
        Task<int> AddTnbwochecklisttype(Tnbwochecklisttype d);
        Task DeleteTnbwochecklisttype(Tnbwochecklisttype d);
        Task UpdateTnbwochecklisttype(Tnbwochecklisttype d);
        Task ResetTnbwochecklisttype();
        Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeByGroup(int id);
        Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeByWO(string tnbwonum);
    }
}
