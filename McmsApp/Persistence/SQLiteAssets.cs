using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteAsset : IAsset
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteAsset()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Asset>();
        }

        public async Task AddAsset(Asset d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Asset ID: {0}", d.assetid);
            });
        }

        public async Task DeleteAsset(Asset d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task<List<Asset>> GetAssetByWonum(string wonum)
        {
            var query = _connection.Table<Asset>().Where(x => x.wonum == wonum);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<Asset> GetAsset(int id)
        {
            string where = $" assetid='{id}'";
            return await _connection.FindWithQueryAsync<Asset>($"SELECT * FROM Asset WHERE{where} ");
        }

        public async Task UpdateAsset(Asset d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task ResetAsset()
        {
            await _connection.DropTableAsync<Asset>();
            await _connection.CreateTableAsync<Asset>();
        }

        public async Task<int?> CountAsset(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Asset where wonum='{wonum}'");

            return count;
        }
    }
}
