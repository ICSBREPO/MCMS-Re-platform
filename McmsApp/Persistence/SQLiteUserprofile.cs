using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using McmsApp.Models;
using SQLite;

namespace McmsApp.Persistence
{
    public class SQLiteUserprofile : IUserProfile
    {
        private SQLiteAsyncConnection _connection { get; set; }

        public SQLiteUserprofile()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mcms.db3");
            _connection = new SQLiteAsyncConnection(databasePath);
            _connection.CreateTableAsync<UserProfile>();
        }

        public async Task<List<UserProfile>> GetUserProfileAsync()
        {
            var query = _connection.Table<UserProfile>();
            var result = await query.ToListAsync();
            return result;
        }

        public async Task ResetUserProfile()
        {
            await _connection.DropTableAsync<UserProfile>();
            await _connection.CreateTableAsync<UserProfile>();
        }

        public async Task<UserProfile> GetUserProfile(int id)
        {
            string where = $" personuid='{id}'";
            return await _connection.FindWithQueryAsync<UserProfile>($"SELECT * FROM UserProfile WHERE{where} ");
        }

        public async Task AddUserProfile(UserProfile d)
        {
            await _connection.InsertAsync(d).ContinueWith((t) =>
            {
                //Console.WriteLine("New UserProfile ID: {0}", d.value);
            });
        }

        public async Task DeleteUserProfile(UserProfile d)
        {
            await _connection.DeleteAsync(d);
        }


        public async Task UpdateUserProfile(UserProfile d)
        {
            await _connection.UpdateAsync(d);
        }

        public async Task<UserProfile> GetUserProfileByUserName(string username)
        {
            string where = $" loginID='{username}'";
            return await _connection.FindWithQueryAsync<UserProfile>($"SELECT * FROM UserProfile WHERE{where} ");
        }

        public async Task<UserProfile> LoginOffline(string username, string password)
        {
            string where = $" loginID='{username}' AND password='{password}'";
            return await _connection.FindWithQueryAsync<UserProfile>($"SELECT * FROM UserProfile WHERE{where} ");
        }
    }
}
