using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTnbmetergroup : ITnbmetergroup
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbmetergroup()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Metergroup>();
        }

        public async Task<List<Metergroup>> GetTnbmetergroupAsync()
        {
            var query = _connection.Table<Metergroup>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbmetergroup()
        {
            await _connection.DropTableAsync<Metergroup>();
            await _connection.CreateTableAsync<Metergroup>();
        }

        public async Task<Metergroup> GetTnbmetergroup(int id)
        {
            string where = $" metergroupid={id}";
            return await _connection.FindWithQueryAsync<Metergroup>($"SELECT * FROM Metergroup WHERE{where} ");
        }

        public async Task AddTnbmetergroup(Metergroup d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Metergroup ID: {0}", d.metergroupid);
            });
        }

        public async Task DeleteTnbmetergroup(Metergroup d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbmetergroup(Metergroup d)
        {
            await _connection.UpdateAsync(d);
        }
    }
}
