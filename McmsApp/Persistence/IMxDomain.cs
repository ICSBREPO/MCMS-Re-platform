﻿using McmsApp.Models;

namespace McmsApp.Persistence
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
