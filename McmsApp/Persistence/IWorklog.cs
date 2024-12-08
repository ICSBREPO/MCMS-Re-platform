﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mcms.Models;

namespace mcms.Persistence
{
    public interface IWorklog
    {
        Task<List<Worklog>> GetWorklogByWonum(string wonum);
        Task<Worklog> GetWorklog(int id);
        Task AddWorklog(Worklog d);
        Task DeleteWorklog(Worklog d);
        Task UpdateWorklog(Worklog d);
        Task ResetWorklog();
        Task<int?> CountWorklog(string wonum);
    }
}