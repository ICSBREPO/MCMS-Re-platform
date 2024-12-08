using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using mcms.Models;
using SQLite;

namespace mcms.Persistence
{
    public class SQLitePlusgaudline : IPlusgaudline
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLitePlusgaudline()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Plusgaudline>();
        }

        public async Task<List<Plusgaudline>> GetPlusgaudlineAsync()
        {
            var query = _connection.Table<Plusgaudline>();

            var result = await query.ToListAsync();

            return result;
        }

        public async Task ResetPlusgaudline()
        {
            await _connection.DropTableAsync<Plusgaudline>();
            await _connection.CreateTableAsync<Plusgaudline>();
        }

        public async Task<Plusgaudline> GetPlusgaudline(int id)
        {
            string where = $" id={id}";
            return await _connection.FindWithQueryAsync<Plusgaudline>($"SELECT * FROM Plusgaudline WHERE{where} ");
        }

        public async Task AddPlusgaudline(Plusgaudline d)
        {
            var result = await _connection.InsertAsync(d);/*.ContinueWith((t) =>
            {
                Console.WriteLine("New Plusgaudline ID: {0}", d.tnbsqaremarks);
            });*/
        }

        public async Task DeletePlusgaudline(Plusgaudline d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdatePlusgaudline(Plusgaudline d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task DeleteByTemplate(bool istemplate)
        {
            /*try
            {*/
                var query = _connection.Table<Plusgaudline>().Where(v => v.template == istemplate);
                await query.DeleteAsync();

           /* }
            catch(Exception e)
            {
                await _connection.CreateTableAsync<Plusgaudline>().ContinueWith((t) =>
            {
                Console.WriteLine("New Plusgaudline ID: {0}", d.tnbsqaremarks);
            });
                var query = _connection.Table<Plusgaudline>().Where(v => v.template == istemplate);
                await query.DeleteAsync();

            }*/

            
        }

        public async Task<List<Plusgaudline>> GetPlusgaudlineBySQA(int? id)
        {
            var query = _connection.Table<Plusgaudline>().Where(x => x.sqaid == id);

            var result = await query.ToListAsync();

            return result;
        }
    }
}
