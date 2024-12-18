using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTheme: ITheme
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTheme()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Theme>();
        }

        public async Task<int> CountTheme()
        {
            var query = _connection.Table<Theme>();
            var result = await query.ToListAsync();
            return result.Count;
        }

        public async Task<List<Theme>> GetTheme()
        {
            var query = _connection.Table<Theme>();
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<Theme> GetThemeId(int id)
        {
            string where = $" id='{id}'";
            return await _connection.FindWithQueryAsync<Theme>($"SELECT * FROM Theme WHERE{where} ");
        }

        public async Task AddTheme(Theme d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Theme ID: {0}", d.title);
            });
        }

        public async Task DeleteTheme(Theme d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTheme(Theme d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task ResetTheme()
        {
            await _connection.DropTableAsync<Theme>();
            await _connection.CreateTableAsync<Theme>();
        }

        public async Task ApplyTheme(int id, Boolean apply)
        {
            //string where = $" id='{id}'";
            //string clause = $" applied='{apply}'";
            //await _connection.ExecuteAsync($"UPDATE Theme SET {clause} WHERE{where}");
            await _connection.ExecuteAsync("UPDATE Theme SET applied = ? Where id = ?", id, apply);

        }
    }
}
