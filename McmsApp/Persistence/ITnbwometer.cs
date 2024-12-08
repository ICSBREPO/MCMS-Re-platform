using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
{
    public interface ITnbwometer
    {
        Task<List<Tnbwometer>> GetTnbwometerAsync();
        Task<Tnbwometer> GetTnbwometer(int id);
        Task AddTnbwometer(Tnbwometer d);
        Task DeleteTnbwometer(Tnbwometer d);
        Task UpdateTnbwometer(Tnbwometer d);
        Task ResetTnbwometer();
        Task<List<Tnbwometer>> GetTnbwometerByGroup(int id);
        Task<int> GetCountMeterReading(int id);
    }
}
