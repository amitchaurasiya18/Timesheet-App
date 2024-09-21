using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Data;
using TimeSheetApp.Models;
using TimeSheetApp.Services;

namespace TimeSheetApp.Repository
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly TimesheetDBContext _dbContext;
        private readonly IUserService _userService;

        public TimesheetRepository(TimesheetDBContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task<Timesheet> Add(Timesheet timesheet)
        {
            await _dbContext.Timesheets.AddAsync(timesheet);
            await _dbContext.SaveChangesAsync();
            return timesheet;
        }

        public async Task Delete(int id)
        {
            var timesheet = await _dbContext.Timesheets.FindAsync(id);
            if (timesheet != null)
            {
                _dbContext.Timesheets.Remove(timesheet);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Timesheet>> GetAll()
        {
            var userId = _userService.GetCurrentUserId();
            return await _dbContext.Timesheets.Include(t => t.Activities)
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task Update(Timesheet timesheet)
        {
            _dbContext.Timesheets.Update(timesheet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Timesheet> GetById(int timesheetId)
        {
            return await _dbContext.Timesheets.Include(t => t.Activities).SingleOrDefaultAsync(s => s.TimesheetId == timesheetId);
        }
    }
}
