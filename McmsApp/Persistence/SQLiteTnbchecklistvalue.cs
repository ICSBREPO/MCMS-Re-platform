using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTnbchecklistvalue : IChecklistVal
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLiteTnbchecklistvalue()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");

            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbchecklistvalue>();
        }

        public async Task AddValue(Tnbchecklistvalue d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {

            });
        }

        public async Task DeleteValue(Tnbchecklistvalue d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task<Tnbchecklistvalue> GetValue(int id)
        {
            return await _connection.FindAsync<Tnbchecklistvalue>(id);
        }

        public async Task<List<Tnbchecklistvalue>> GetValueAsync(string checklist)
        {
            var result = await _connection.QueryAsync<Tnbchecklistvalue>($"SELECT * FROM Tnbchecklistvalue WHERE tnbchecklist='{checklist}' ORDER BY tnbchecklistvalueid");
            return result;
        }

        public async Task ResetValue()
        {
            await _connection.DropTableAsync<Tnbchecklistvalue>();
            await _connection.CreateTableAsync<Tnbchecklistvalue>();
        }

        public async Task UpdateValue(Tnbchecklistvalue d)
        {
            await _connection.UpdateAsync(d);
        }
    }
}
