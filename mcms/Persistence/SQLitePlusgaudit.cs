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
    public class SQLitePlusgaudit : IPlusgaudit
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLitePlusgaudit()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Plusgaudit>();
        }

        public async Task<List<Plusgaudit>> GetPlusgauditAsync(bool template)
        {
            string username = await SecureStorage.GetAsync("username");
            var query = _connection.Table<Plusgaudit>().Where(x => x.template == template).Where(x => x.completedby == username);
            if (template)
            {
                query = _connection.Table<Plusgaudit>().Where(x => x.template == template);
            }
            var result = await query.ToListAsync();
            return result;
        }

        public async Task ResetPlusgaudit()
        {
            await _connection.DropTableAsync<Plusgaudit>();
            await _connection.CreateTableAsync<Plusgaudit>();
        }

        public async Task<Plusgaudit> GetPlusgaudit(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Plusgaudit>($"SELECT * FROM Plusgaudit WHERE{where} ");
        }

        public async Task<int> AddPlusgaudit(Plusgaudit d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New SqaList ID: {0}", d.plusgauditid);
                return d.id;
            });
            return result;
        }

        public async Task DeletePlusgaudit(Plusgaudit d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdatePlusgaudit(Plusgaudit d)
        {
            Debug.WriteLine("Is Pending Plusgaudit : " + d.pendingupload);
            await _connection.UpdateAsync(d);
        }

        public async Task DeleteByTemplate(bool istemplate)
        {
            var query = _connection.Table<Plusgaudit>().Where(v => v.template == istemplate);
            await query.DeleteAsync();
        }

        public async Task<List<Plusgaudit>> GetPlusgauditByWO(string wonum)
        {
            var query = _connection.Table<Plusgaudit>().Where(x => x.tnbwonum == wonum);

            var result = await query.ToListAsync();

            return result;
        }

        public async Task<int?> CountSQA(string wonum)
        {
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Plusgaudit where tnbwonum='{wonum}'");

            return count;
        }

        public async Task<int?> LastSQA()
        {
            int? id = await _connection.ExecuteScalarAsync<int>($"select id from Plusgaudit order by id desc limit 1");

            return id;
        }
    }
}