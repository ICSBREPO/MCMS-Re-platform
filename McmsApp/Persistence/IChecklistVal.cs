using mcms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public interface IChecklistVal
    {
        Task<List<Tnbchecklistvalue>> GetValueAsync(string checklist);
        Task<Tnbchecklistvalue> GetValue(int id);
        Task AddValue(Tnbchecklistvalue d);
        Task UpdateValue(Tnbchecklistvalue d);
        Task DeleteValue(Tnbchecklistvalue d);
        Task ResetValue();
    }
}
