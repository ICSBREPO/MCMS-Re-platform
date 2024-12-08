using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;
using Xamarin.Essentials;

namespace mcms.Persistence
{
    public class SQLiteWorkorder : IWorkorder
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public string username { get; set; }

        public SQLiteWorkorder()
        {
           _ = loadusername();
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            Debug.WriteLine("database path is : "+databasePath);
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Workorder>();
        }
        async Task loadusername()
        {
            username = await SecureStorage.GetAsync("username");
        }
        public async Task<List<WOChart>> GetWOChart(string attr)
        {
            string user = await SecureStorage.GetAsync("username");
            var result = await _connection.QueryAsync<WOChart>($"SELECT count(wonum) as Count, {attr} as Name FROM Workorder where username='{user}' group by {attr}");
            return result;
        }

        public async Task<List<Workorder>> GetSearchWorkorder(string search)
        {
            string where;
            where = $"where username='{username}' and status like '%{search}%' ";
            where += $"or worktype like '%{search}%' ";
            where += $"or wonum like '%{search}%' ";
            where += $"or description like '%{search}%' ";
            where += $"or tnbworkscope like '%{search}%' ";

            var result = await _connection.QueryAsync<Workorder>($"SELECT * FROM Workorder {where}");
            return result;
        }

        public async Task<List<Workorder>> GetWorkorderInprg()
        {
            var result = await _connection.QueryAsync<Workorder>($"SELECT * FROM Workorder WHERE username='{username}' and status='COMP'");
            return result;
        }

        public async Task<List<Workorder>> GetWorkorderAppr()
        {
            var result = await _connection.QueryAsync<Workorder>($"SELECT * FROM Workorder WHERE  username='{username}' and status='APPR'");
            return result;
        }

        public async Task<List<Workorder>> GetWorkorderAsync()
        {
            var query = _connection.Table<Workorder>().Where(x => x.username == username);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetWorkorder()
        {
            await _connection.DropTableAsync<Workorder>();
            await _connection.CreateTableAsync<Workorder>();
        }

        public async Task<Workorder> GetWorkorder(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Workorder>($"SELECT * FROM Workorder WHERE{where} ");
        }

        public async Task<Workorder> GetParentWorkorder(string parent)
        {
            string where = $" wonum={parent}";
            return await _connection.FindWithQueryAsync<Workorder>($"SELECT * FROM Workorder WHERE{where} ");
        }

        public async Task<List<Workorder>> GetFilterWorkorder(string where)
        {
            var result = await _connection.QueryAsync<Workorder>($"SELECT * FROM Workorder {where}");
            return result;
        }

        public async Task<int> AddWorkorder(Workorder d)
        {
            int id = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                //Console.WriteLine("New Workorder ID: {0}", d.value);
                return d.id;
            });
            return id;
        }

        public async Task DeleteWorkorder(Workorder d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateWorkorder(Workorder d)
        {
            Debug.WriteLine("Is Pending workorder " + d.pendingupdate);
            await _connection.UpdateAsync(d);
        }

        public async Task<List<Distinct>> GetListDistinct(string attr)
        {
            var result = await _connection.QueryAsync<Distinct>($"SELECT distinct {attr} as value FROM Workorder where username='{username}'");
            return result;
        }

        public async Task<List<Workorder>> GetChildWorkorders(string wonum)
        {
            var query = _connection.Table<Workorder>().Where(x => x.parent == wonum);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int> CountWorkorder()
        {
            var query = _connection.Table<Workorder>().Where(x => x.username == username);
            var result = await query.ToListAsync();
            return result.Count;
        }

        public async Task<int?> CountChild(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Workorder where parent='{wonum}'");

            return count;
        }
    }
}
