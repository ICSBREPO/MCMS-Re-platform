using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;

namespace mcms.Persistence
{
    public class SQLiteCompanies : ICompanies
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLiteCompanies()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");

            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Companies>();
        }

        public async Task AddCompanies(Companies d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                
            });
        }

        public async Task DeleteCompanies(Companies d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task<Companies> GetCompanies(int Id)
        {
            return await _connection.FindAsync<Companies>(Id);
        }

        public async Task<List<Companies>> GetCompaniesAsync()
        {
            var query = _connection.Table<Companies>();
            var result = await query.ToListAsync();
            return result;
        }

        public async Task ResetCompanies()
        {
            await _connection.DropTableAsync<Companies>();
            await _connection.CreateTableAsync<Companies>();
        }

        public async Task UpdateCompanies(Companies d)
        {
            await _connection.UpdateAsync(d);
        }

    }
}
