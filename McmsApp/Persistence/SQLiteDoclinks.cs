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
    public class SQLiteDoclinks : IDoclinks
    {
        private SQLiteAsyncConnection _connection { get; set; }
        public SQLiteDoclinks()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<Doclinks>();
        }

        public async Task<List<Doclinks>> GetDoclinksFilter(int refid, string ownertable, bool isdelete = false, string reference="0")
        {

           

            var query = _connection.Table<Doclinks>().Where(x => x.refid == refid).Where(x => x.ownertable == ownertable).Where(x=>x.isdelete==isdelete);
            if (reference != "0")
            {
                query = query.Where(x => x.reference == reference);
             }
             if (reference.Contains("*0"))
            {
           
                reference = "0";
                query = query.Where(x => x.reference == reference);

            }

        var result = await query.ToListAsync();

            return result;
        }

        public async Task<int?> AddDoclinks(Doclinks d)
        {
            var result = await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                Console.WriteLine("New DoclinksList ID: {0}", d.reference);
                return d.id;
            });
            return result;
        }

        public async Task DeleteDoclinks(Doclinks d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task ResetDoclinks()
        {
            await _connection.DropTableAsync<Doclinks>();
            await _connection.CreateTableAsync<Doclinks>();
        }

        public async Task UpdateDoclinks(Doclinks d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<int?> CountDoclinks(int refid, string ownertable, string reference="0")
        {
            /*if (reference != "0")
            {*/
                reference = $" and reference='{reference}'";
            /*}
            else if(reference=="0" || reference=="")
            {
                reference = "";
            }*/
            int? count = await _connection.ExecuteScalarAsync<int>($"select count(*) from Doclinks where refid={refid} and ownertable='{ownertable}'{reference}");

            return count;
        }
    }
}
