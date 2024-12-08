using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;

namespace mcms.Persistence
{
    public class SQLiteMxDomain : IMxDomain
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLiteMxDomain()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");

            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<MxDomain>();
        }

        public async Task AddMxDomain(MxDomain d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Domain ID: {0} {1}", d.domainid, d.value);
            });
        }

        public async Task DeleteMxDomain(MxDomain d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task<MxDomain> GetMxDomain(int Id)
        {
            return await _connection.FindAsync<MxDomain>(Id);
        }

        public async Task<List<MxDomain>> GetMxDomainAsync(string domainid)
        {
            var result = await _connection.QueryAsync<MxDomain>($"SELECT * FROM MxDomain WHERE domainid='{domainid}' GROUP BY value");
            return result;
        }

        public async Task ResetMxDomain()
        {
            await _connection.DropTableAsync<MxDomain>();
            await _connection.CreateTableAsync<MxDomain>();
        }

        public async Task UpdateMxDomain(MxDomain d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<MxDomain>> GetMxDomain(string domainid)
        {
            var query = _connection.Table<MxDomain>().Where(x => x.domainid == domainid);

            var result = await query.ToListAsync();

            return result;
        }
    }
}
