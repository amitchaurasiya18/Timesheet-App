using TimeSheetApp.Models;

namespace TimeSheetApp.Repository
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        User GetUser(Login login);
    }
}
