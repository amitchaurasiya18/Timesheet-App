using TimeSheetApp.Models;

namespace TimeSheetApp.Services
{
    public interface IUserService
    {
        Task<User> AddUser(User user);
        string Login(Login login);
        int GetCurrentUserId();
    }
}
