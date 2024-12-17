using McmsApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
    public class SQLiteTnbwochecklistsignature : ITnbwochecklistsignature
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbwochecklistsignature()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwochecklistsignature>();
        }

        public async Task<List<Tnbwochecklistsignature>> GetTnbwochecklistsignatureAsync()
        {
            var query = _connection.Table<Tnbwochecklistsignature>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwochecklistsignature()
        {
            await _connection.DropTableAsync<Tnbwochecklistsignature>();
            await _connection.CreateTableAsync<Tnbwochecklistsignature>();
        }

        public async Task<Tnbwochecklistsignature> GetTnbwochecklistsignature(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklistsignature>($"SELECT * FROM Tnbwochecklistsignature WHERE{where} ");
        }

        public async Task<Tnbwochecklistsignature> GetTnbwochecklistsignatureFromPlusgaudit(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklistsignature>($"SELECT * FROM Tnbwochecklistsignature WHERE{where} ");
        }

        public async Task<int?> AddTnbwochecklistsignature(Tnbwochecklistsignature d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Checklisttype ID: {0}", d.id);
                return d.id;
            });
            return result;
        }

        public async Task DeleteTnbwochecklistsignature(Tnbwochecklistsignature d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbwochecklistsignature(Tnbwochecklistsignature d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbwochecklistsignature>> GetTnbwochecklistsignatureByGroup(int id)
        {
            var query = _connection.Table<Tnbwochecklistsignature>().Where(x => x.tnbownerid == id);

            var result = await query.ToListAsync();

            return result;
        }



    }
}
