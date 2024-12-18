using McmsApp.Models;

namespace McmsApp.Persistence
{
    public interface IUserProfile
    {
        Task<List<UserProfile>> GetUserProfileAsync();
        Task<UserProfile> GetUserProfile(int id);
        Task AddUserProfile(UserProfile d);
        Task DeleteUserProfile(UserProfile d);
        Task UpdateUserProfile(UserProfile d);
        Task ResetUserProfile();
        Task<UserProfile> GetUserProfileByUserName(string username);
        Task<UserProfile> LoginOffline(string username, string password);
    }
}
