﻿using McmsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
    public interface ITnbpsiwochecklisttype
    {
        Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeAsync();
        Task<Tnbpsiwochecklisttype> GetTnbpsiwochecklisttype(int id);
        Task<int> AddTnbpsiwochecklisttype(Tnbpsiwochecklisttype d);
        Task<int?> CountChecklist(string wonum);
        Task DeleteTnbpsiwochecklisttype(Tnbpsiwochecklisttype d);
        Task UpdateTnbpsiwochecklisttype(Tnbpsiwochecklisttype d);
        Task ResetTnbpsiwochecklisttype();
        Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeByGroup(int id);
        Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeByWO(string tnbwonum);
    }
}
