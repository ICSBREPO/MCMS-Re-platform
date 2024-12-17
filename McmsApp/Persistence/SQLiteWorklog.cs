using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteWorklog: IWorklog
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteWorklog()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Worklog>();
        }

        public async Task AddWorklog(Worklog d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New Worklog ID: {0}", d.id);
            });
        }

        public async Task DeleteWorklog(Worklog d)
        {
            await _connection.DeleteAsync(d);
        }

        public async Task<List<Worklog>> GetWorklogByWonum(string wonum)
        {
            var query = _connection.Table<Worklog>().Where(x => x.recordkey == wonum);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<Worklog> GetWorklog(int id)
        {
            string where = $" id='{id}'";
            return await _connection.FindWithQueryAsync<Worklog>($"SELECT * FROM Worklog WHERE{where} ");
        }

        public async Task UpdateWorklog(Worklog d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task ResetWorklog()
        {
            await _connection.DropTableAsync<Worklog>();
            await _connection.CreateTableAsync<Worklog>();
        }

        public async Task<int?> CountWorklog(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Worklog where recordkey='{wonum}'");

            return count;
        }
    }
}
