using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;
using Xamarin.Essentials;

namespace mcms.Persistence
{
    public class SQLitePermitWork : IPermitWork
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLitePermitWork()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<PermitWork>();
        }

        public async Task<List<PermitWork>> GetPermitWorkWO(string wonum)
        {

            var query = _connection.Table<PermitWork>().Where(x => x.tnbwonum == wonum);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<List<PermitWork>> GetPermitWorkUpload()
        {
            string username = await SecureStorage.GetAsync("username");
            var query = _connection.Table<PermitWork>().Where(x => x.pendingupload).Where(y => y.username == username);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int> AddPermitWork(PermitWork d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New PermitWorkList ID: {0}", d.id);
                return d.id;
            });
            return result;
        }

        public async Task DeletePermitWork(PermitWork d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task ResetPermitWork()
        {
            await _connection.DropTableAsync<PermitWork>();
            await _connection.CreateTableAsync<PermitWork>();
        }

        public async Task UpdatePermitWork(PermitWork d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<int?> CountPermit(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from PermitWork where tnbwonum='{wonum}'");

            return count;
        }
    }
}
