using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteTnbsignature : ITnbsignature
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbsignature()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbsignature>();
        }

        public async Task<List<Tnbsignature>> GetTnbsignatureAsync()
        {
            var query = _connection.Table<Tnbsignature>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbsignature()
        {
            await _connection.DropTableAsync<Tnbsignature>();
            await _connection.CreateTableAsync<Tnbsignature>();
        }

        public async Task<Tnbsignature> GetTnbsignature(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbsignature>($"SELECT * FROM Tnbsignature WHERE{where} ");
        }

        public async Task<Tnbwochecklistsignature> GetTnbwochecklistsignatureFromPlusgaudit(int id)
        {
            string where = $" tnbownerid={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklistsignature>($"SELECT * FROM Tnbwochecklistsignature WHERE{where} ");
        }

        public async Task<List<Tnbsignature>> GetTnbsignaturefromPlusgauditList(int id,bool isrefid=false)
        {

            if(isrefid)
            {
                string where = $" refid={id}";
                var query = _connection.Table<Tnbsignature>().Where(x => x.refid == id || x.tnbownerid == id);
                var result = await query.ToListAsync();
                return result;
            }
            else
            {
                string where = $" tnbownerid={id}";
                var query = _connection.Table<Tnbsignature>().Where(x => x.tnbownerid == id);
                var result = await query.ToListAsync();
                return result;
            }
         
            

            

            

        }





        public async Task<int?> AddTnbsignature(Tnbsignature d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Checklisttype ID: {0}", d.id);
                return d.id;
            });
            return result;
        }

        public async Task DeleteTnbsignature(Tnbsignature d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbsignature(Tnbsignature d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbsignature>> GetTnbsignatureByGroup(int id, string ownertable)
        {
            var query = _connection.Table<Tnbsignature>().Where(x => x.refid == id && x.tnbownertable == ownertable);

            var result = await query.ToListAsync();

            return result;
        }



    }
}
