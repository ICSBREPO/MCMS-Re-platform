using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
{
    public interface IMxDomain
    {
        Task<List<MxDomain>> GetMxDomainAsync(string domainid);
        Task<List<MxDomain>> GetMxDomain(string domainid);
        Task<MxDomain> GetMxDomain(int Id);
        Task AddMxDomain(MxDomain d);
        Task UpdateMxDomain(MxDomain d);
        Task DeleteMxDomain(MxDomain d);
        Task ResetMxDomain();
    }
}
