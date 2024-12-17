using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteAssetSpec : IAssetSpec
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteAssetSpec()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<AssetSpec>();
        }

        public async Task<List<AssetSpec>> GetAssetSpecAsync()
        {
            var query = _connection.Table<AssetSpec>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetAssetSpec()
        {
            await _connection.DropTableAsync<AssetSpec>();
            await _connection.CreateTableAsync<AssetSpec>();
        }

        public async Task<AssetSpec> GetAssetSpec(int id)
        {
            string where = $" assetattrid={id}";
            return await _connection.FindWithQueryAsync<AssetSpec>($"SELECT * FROM AssetSpec WHERE{where} ");
        }

        public async Task AddAssetSpec(AssetSpec d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New AssetSpec ID: {0}", d.assetattrid);
            });
        }

        public async Task DeleteAssetSpec(AssetSpec d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateAssetSpec(AssetSpec d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<int?> CountAssetSpec(int id)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from AssetSpec where workorderid={id}");

            return count;
        }


        public async Task<List<AssetSpec>> GetAssetSpecByWO(int id)
        {
            var query = _connection.Table<AssetSpec>().Where(x => x.workorderid == id);

            var result = await query.ToListAsync();

            return result;
        }

    }
}
