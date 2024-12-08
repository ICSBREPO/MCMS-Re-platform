using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
{
    public interface IPlusgaudline
    {
        Task<List<Plusgaudline>> GetPlusgaudlineAsync();
        Task<Plusgaudline> GetPlusgaudline(int id);
        Task AddPlusgaudline(Plusgaudline d);
        Task DeleteByTemplate(bool istemplate);
        Task DeletePlusgaudline(Plusgaudline d);
        Task UpdatePlusgaudline(Plusgaudline d);
        Task ResetPlusgaudline();
        Task<List<Plusgaudline>> GetPlusgaudlineBySQA(int? id);
    }
}
