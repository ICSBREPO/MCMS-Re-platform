using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IPermitWork
    {
        Task<List<PermitWork>> GetPermitWorkWO(string wonum);
        Task<List<PermitWork>> GetPermitWorkUpload();
        Task<int> AddPermitWork(PermitWork d);
        Task<int?> CountPermit(string wonum);
        Task UpdatePermitWork(PermitWork d);
        Task DeletePermitWork(PermitWork d);
        Task ResetPermitWork();
    }
}