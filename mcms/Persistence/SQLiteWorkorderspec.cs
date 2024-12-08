using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;

namespace mcms.Persistence
{
    public class SQLiteWorkorderspec : IWorkorderspec
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteWorkorderspec()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Workorderspec>();
        }

        public async Task<List<Workorderspec>> GetWorkorderspecAsync()
        {
            var query = _connection.Table<Workorderspec>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetWorkorderspec()
        {
            await _connection.DropTableAsync<Workorderspec>();
            await _connection.CreateTableAsync<Workorderspec>();
        }

        public async Task<Workorderspec> GetWorkorderspec(int id)
        {
            string where = $" assetattrid={id}";
            return await _connection.FindWithQueryAsync<Workorderspec>($"SELECT * FROM Workorderspec WHERE{where} ");
        }

        public async Task AddWorkorderspec(Workorderspec d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Workorderspec ID: {0}", d.assetattrid);
            });
        }

        public async Task DeleteWorkorderspec(Workorderspec d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateWorkorderspec(Workorderspec d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<int?> CountWOSpec(int id)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Workorderspec where refobjectid={id}");

            return count;
        }


        public async Task<List<Workorderspec>> GetWorkorderspecByWO(int id)
        {
            var query = _connection.Table<Workorderspec>().Where(x => x.refobjectid == id);

            var result = await query.ToListAsync();

            return result;
        }

    }
}
