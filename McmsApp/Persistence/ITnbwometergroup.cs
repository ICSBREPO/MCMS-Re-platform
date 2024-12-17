using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ITnbwometergroup
    {
        Task<List<Tnbwometergroup>> GetTnbwometergroupAsync();
        Task<Tnbwometergroup> GetTnbwometergroup(int id);
        Task AddTnbwometergroup(Tnbwometergroup d);
        Task DeleteTnbwometergroup(Tnbwometergroup d);
        Task UpdateTnbwometergroup(Tnbwometergroup d);
        Task ResetTnbwometergroup();
        Task<List<Tnbwometergroup>> GetTnbwometergroupByWO(string wonum);
        Task<int?> CountMeter(string wonum);
    }
}
