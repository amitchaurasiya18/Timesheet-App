using TimeSheetApp.Data;
using TimeSheetApp.Models;

namespace TimeSheetApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TimesheetDBContext _dbContext;

        public UserRepository(TimesheetDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public User GetUser(Login login)
        {
            if (login.Username != null && login.Password != null)
            {
                var user = _dbContext.Users.SingleOrDefault(u => u.Username == login.Username);
                return user;
            }
            return null;
        }
    }
}
