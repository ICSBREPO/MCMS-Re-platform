using McmsApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace McmsApp.Persistence
{
    public class SQLiteTnbpsiwochecklisttype : ITnbpsiwochecklisttype
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbpsiwochecklisttype()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbpsiwochecklisttype>();
        }

        public async Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeAsync()
        {
            var query = _connection.Table<Tnbpsiwochecklisttype>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbpsiwochecklisttype()
        {
            await _connection.DropTableAsync<Tnbpsiwochecklisttype>();
            await _connection.CreateTableAsync<Tnbpsiwochecklisttype>();
        }

        public async Task<Tnbpsiwochecklisttype> GetTnbpsiwochecklisttype(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbpsiwochecklisttype>($"SELECT * FROM Tnbpsiwochecklisttype WHERE{where} ");
        }

        public async Task<int> AddTnbpsiwochecklisttype(Tnbpsiwochecklisttype d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Checklisttype ID: {0}", d.tnbpsiwochecklisttypeid);
                return d.tnbpsiwochecklisttypeid;
            });
            return result;
        }

        public async Task DeleteTnbpsiwochecklisttype(Tnbpsiwochecklisttype d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbpsiwochecklisttype(Tnbpsiwochecklisttype d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeByGroup(int id)
        {
            var query = _connection.Table<Tnbpsiwochecklisttype>().Where(x => x.tnbwochecklisthdrid == id);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<List<Tnbpsiwochecklisttype>> GetTnbpsiwochecklisttypeByWO(string tnbwonum)
        {
            var query = _connection.Table<Tnbpsiwochecklisttype>().Where(x => x.tnbwonum == tnbwonum).OrderBy(x=>x.tnbsequence);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int?> CountChecklist(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Tnbpsiwochecklisttype where tnbwonum='{wonum}'");

            return count;
        }

    }
}
