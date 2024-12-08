using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;

namespace mcms.Persistence
{
    public class SQLiteTnbwometer : ITnbwometer
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbwometer()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwometer>();
        }

        public async Task<List<Tnbwometer>> GetTnbwometerAsync()
        {
            var query = _connection.Table<Tnbwometer>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwometer()
        {
            await _connection.DropTableAsync<Tnbwometer>();
            await _connection.CreateTableAsync<Tnbwometer>();
        }

        public async Task<Tnbwometer> GetTnbwometer(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwometer>($"SELECT * FROM Tnbwometer WHERE{where} ");
        }

        public async Task AddTnbwometer(Tnbwometer d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Tnbwometer ID: {0}", d.description);
            });
        }

        public async Task DeleteTnbwometer(Tnbwometer d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateTnbwometer(Tnbwometer d)
        {
            await _connection.UpdateAsync(d);
        }


        public async Task<List<Tnbwometer>> GetTnbwometerByGroup(int id)
        {
            var query = _connection.Table<Tnbwometer>().Where(x => x.tnbwometergroupid == id);//.OrderBy(x=>x);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int> GetCountMeterReading(int id)
        {
            var query = _connection.Table<Tnbwometer>().Where(x => x.tnbnewreading == null || x.tnbnewreading == "").Where(x => x.tnbwometergroupid == id);
            var result = await query.ToListAsync();
            return result.Count;
        }
    }
}
