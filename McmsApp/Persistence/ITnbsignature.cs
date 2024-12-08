using mcms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public interface ITnbsignature
    {
        Task<List<Tnbsignature>> GetTnbsignatureAsync();
        Task<Tnbsignature> GetTnbsignature(int id);
        Task<int?> AddTnbsignature(Tnbsignature d);
        Task DeleteTnbsignature(Tnbsignature d);
        Task UpdateTnbsignature(Tnbsignature d);
        Task ResetTnbsignature();
        Task<List<Tnbsignature>> GetTnbsignatureByGroup(int id, string ownertable);
     /*   Task<Tnbsignature> GetTnbsignaturefromPlusgaudit(int plusgauditid);*/
        /*Task<Tnbsignature> GetTnbwochecklistsignatureFromPlusgaudit(int plusgauditid);*/
        Task<List<Tnbsignature>> GetTnbsignaturefromPlusgauditList(int plusgauditid,bool isref);
    }
}
