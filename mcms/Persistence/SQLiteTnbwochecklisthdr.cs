using mcms.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace mcms.Persistence
{
    public class SQLiteTnbwochecklisthdr : ITnbwochecklisthdr
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteTnbwochecklisthdr()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Tnbwochecklisthdr>();
        }

        public async Task<List<Tnbwochecklisthdr>> GetTnbwochecklisthdrAsync()
        {
            var query = _connection.Table<Tnbwochecklisthdr>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetTnbwochecklisthdr()
        {
            await _connection.DropTableAsync<Tnbwochecklisthdr>();
            await _connection.CreateTableAsync<Tnbwochecklisthdr>();
        }

        public async Task<Tnbwochecklisthdr> GetTnbwochecklisthdr(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Tnbwochecklisthdr>($"SELECT * FROM Tnbwochecklisthdr WHERE{where} ");
        }

        public async Task<Tnbwochecklisthdr> GetTnbwochecklisthdrs(string tnbwonum)
        {
            return await _connection.FindWithQueryAsync<Tnbwochecklisthdr>($"SELECT * FROM Tnbwochecklisthdr WHERE tnbwonum={tnbwonum} ");
        }

        public async Task<int?> AddTnbwochecklisthdr(Tnbwochecklisthdr d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Checklisthdr ID: {0}", d.tnbpsiwochecklisttypeid);
                return d.tnbwochecklisthdrid;
            });
            return result;
        }

        public async Task DeleteTnbwochecklisthdr(Tnbwochecklisthdr d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task UpdateTnbwochecklisthdr(Tnbwochecklisthdr d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Tnbwochecklisthdr>> GetTnbwochecklisthdrByGroup(int tnbpsiwochecklisttypeid)
        {
            var query = _connection.Table<Tnbwochecklisthdr>().Where(x => x.tnbpsiwochecklisttypeid == tnbpsiwochecklisttypeid);

            var result = await query.ToListAsync();

            return result;
        }
    }
}
