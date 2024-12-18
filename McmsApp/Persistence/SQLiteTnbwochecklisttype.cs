using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTnbwochecklisttype : ITnbwochecklisttype
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbwochecklisttype()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwochecklisttype>();
        }

        public async Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeAsync()
        {
            var query = _connection.Table<Tnbwochecklisttype>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwochecklisttype()
        {
            await _connection.DropTableAsync<Tnbwochecklisttype>();
            await _connection.CreateTableAsync<Tnbwochecklisttype>();
        }

        public async Task<Tnbwochecklisttype> GetTnbwochecklisttype(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklisttype>($"SELECT * FROM Tnbwochecklisttype WHERE{where} ");
        }

        public async Task<int> AddTnbwochecklisttype(Tnbwochecklisttype d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Checklisttype ID: {0}", d.tnbwochecklisttypeid);
                return d.tnbwochecklisttypeid;
            });
            return result;
        }

        public async Task DeleteTnbwochecklisttype(Tnbwochecklisttype d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbwochecklisttype(Tnbwochecklisttype d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeByGroup(int id)
        {
            var query = _connection.Table<Tnbwochecklisttype>().Where(x => x.tnbpsiwochecklisttypeid == id).OrderBy(x=>x.tnbsequence);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<List<Tnbwochecklisttype>> GetTnbwochecklisttypeByWO(string tnbwonum)
        {
            var query = _connection.Table<Tnbwochecklisttype>().Where(x => x.tnbwonum == tnbwonum);

            var result = await query.ToListAsync();

            return result;
        }

    }
}
