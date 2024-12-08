using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
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
