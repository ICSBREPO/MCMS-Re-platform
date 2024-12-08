using mcms.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public class SQLiteLocations : ILocations
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteLocations()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Locations>();
        }
        public async Task AddLocations(Locations d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Locations ID: {0}", d.locationsid);
            });
        }

        public async Task<int?> CountLocations(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Locations where wonum='{wonum}'");

            return count;
        }

        public async Task DeleteLocations(Locations d)
        {
            await _connection.DeleteAsync(d);
        }

        public Task<Asset> GetLocations(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Locations>> GetLocationsByWonum(string wonum)
        {
            var query = _connection.Table<Locations>().Where(x => x.wonum == wonum);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task ResetLocations()
        {
            await _connection.DropTableAsync<Locations>();
            await _connection.CreateTableAsync<Locations>();
        }

        public async Task UpdateLocations(Locations d)
        {
            await _connection.UpdateAsync(d);
        }
    }
}
