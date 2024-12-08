﻿using System;
using System.Collections.Generic;
using System.Text;
using mcms.Models;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public interface ITnbwochecklist
    {
        Task<List<Tnbwochecklist>> GetTnbwochecklistAsync();
        Task<Tnbwochecklist> GetTnbwochecklist(int id);
        Task AddTnbwochecklist(Tnbwochecklist d);
        Task DeleteTnbwochecklist(Tnbwochecklist d);
        Task UpdateTnbwochecklist(Tnbwochecklist d);
        Task ResetTnbwochecklist();
        Task<List<Tnbwochecklist>> GetTnbwochecklistByGroup(int id);
        Task<List<Tnbwochecklist>> GetTnbwochecklistByWO(string tnbwonum);
    }
}
