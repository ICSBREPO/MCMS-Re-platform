using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface ITheme
    {
        Task<int> CountTheme();
        Task<List<Theme>> GetTheme();
        Task<Theme> GetThemeId(int id);
        Task AddTheme(Theme d);
        Task DeleteTheme(Theme d);
        Task UpdateTheme(Theme d);
        Task ResetTheme();
        Task ApplyTheme(int id, Boolean apply);
    }
}
