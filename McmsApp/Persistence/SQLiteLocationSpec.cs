using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteLocationSpec : ILocationSpec
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteLocationSpec()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<LocationSpec>();
        }

        public async Task<List<LocationSpec>> GetLocationSpecAsync()
        {
            var query = _connection.Table<LocationSpec>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetLocationSpec()
        {
            await _connection.DropTableAsync<LocationSpec>();
            await _connection.CreateTableAsync<LocationSpec>();
        }

        public async Task<LocationSpec> GetLocationSpec(int id)
        {
            string where = $" assetattrid={id}";
            return await _connection.FindWithQueryAsync<LocationSpec>($"SELECT * FROM LocationSpec WHERE{where} ");
        }

        public async Task AddLocationSpec(LocationSpec d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New LocationSpec ID: {0}", d.assetattrid);
            });
        }

        public async Task DeleteLocationSpec(LocationSpec d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateLocationSpec(LocationSpec d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<int?> CountLocationSpec(int id)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from LocationSpec where workorderid={id}");

            return count;
        }


        public async Task<List<LocationSpec>> GetLocationSpecByWO(int id)
        {
            var query = _connection.Table<LocationSpec>().Where(x => x.workorderid == id);

            var result = await query.ToListAsync();

            return result;
        }

    }
}
