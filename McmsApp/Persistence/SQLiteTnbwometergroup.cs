using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTnbwometergroup : ITnbwometergroup
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbwometergroup()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwometergroup>();
        }

        public async Task<List<Tnbwometergroup>> GetTnbwometergroupAsync()
        {
            var query = _connection.Table<Tnbwometergroup>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwometergroup()
        {
            await _connection.DropTableAsync<Tnbwometergroup>();
            await _connection.CreateTableAsync<Tnbwometergroup>();
        }

        public async Task<Tnbwometergroup> GetTnbwometergroup(int id)
        {
            string where = $" tnbwometergroupid={id}";
            return await _connection.FindWithQueryAsync<Tnbwometergroup>($"SELECT * FROM Tnbwometergroup WHERE{where} ");
        }

        public async Task AddTnbwometergroup(Tnbwometergroup d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Tnbwometergroup ID: {0}", d.tnbwometergroupid);
            });
        }

        public async Task DeleteTnbwometergroup(Tnbwometergroup d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateTnbwometergroup(Tnbwometergroup d)
        {
            await _connection.UpdateAsync(d);
        }


        public async Task<List<Tnbwometergroup>> GetTnbwometergroupByWO(string wonum)
        {
            var query = _connection.Table<Tnbwometergroup>().Where(x => x.tnbwonum == wonum).OrderByDescending(x=>x.tnbsequence);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int?> CountMeter(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Tnbwometergroup where tnbwonum='{wonum}'");

            return count;
        }
    }
}
