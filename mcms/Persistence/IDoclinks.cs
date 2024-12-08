using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
{
    public interface IDoclinks
    {
        Task<List<Doclinks>> GetDoclinksFilter(int refid,string ownertable,bool isdelete = false, string reference = "0");
        Task<int?> AddDoclinks(Doclinks d);
        Task<int?> CountDoclinks(int refid,string ownertable,string reference = "0");
        Task UpdateDoclinks(Doclinks d);
        Task DeleteDoclinks(Doclinks d);
        Task ResetDoclinks();
    }
}