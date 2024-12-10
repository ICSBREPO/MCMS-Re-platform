using McmsApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
   public class SQLiteTnbwochecklist : ITnbwochecklist
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLiteTnbwochecklist()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwochecklist>();
        }

        public async Task<List<Tnbwochecklist>> GetTnbwochecklistAsync()
        {
            var query = _connection.Table<Tnbwochecklist>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwochecklist()
        {
            await _connection.DropTableAsync<Tnbwochecklist>();
            await _connection.CreateTableAsync<Tnbwochecklist>();
        }

        public async Task<Tnbwochecklist> GetTnbwochecklist(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklist>($"SELECT * FROM Tnbwochecklist WHERE{where} ");
        }

        public async Task AddTnbwochecklist(Tnbwochecklist d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Tnbwochecklist ID: {0}", d.tnbdescription);
            });
        }

        public async Task DeleteTnbwochecklist(Tnbwochecklist d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbwochecklist(Tnbwochecklist d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbwochecklist>> GetTnbwochecklistByGroup(int id)
        {
            var query = _connection.Table<Tnbwochecklist>().Where(x => x.tnbwochecklisttypeid == id).OrderBy(x => x.tnbsequence);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<List<Tnbwochecklist>> GetTnbwochecklistByWO(string tnbwonum)
        {
            var query = _connection.Table<Tnbwochecklist>().Where(x => x.tnbwonum == tnbwonum);

            var result = await query.ToListAsync();

            return result;
        }
    }
}
