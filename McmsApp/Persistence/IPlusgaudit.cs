using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IPlusgaudit
    {
        Task<List<Plusgaudit>> GetPlusgauditAsync(bool template);
        Task<Plusgaudit> GetPlusgaudit(int id);
        Task<int> AddPlusgaudit(Plusgaudit d);
        Task<int?> CountSQA(string wonum);
        Task<int?> LastSQA();
        Task DeleteByTemplate(bool istemplate);
        Task DeletePlusgaudit(Plusgaudit d);
        Task UpdatePlusgaudit(Plusgaudit d);
        Task ResetPlusgaudit();
        Task<List<Plusgaudit>> GetPlusgauditByWO(string wonum);
    }
}