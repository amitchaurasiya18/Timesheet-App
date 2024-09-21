using TimeSheetApp.Models;

namespace TimeSheetApp.Services
{
    public interface ITimesheetService
    {
        Task<IEnumerable<Timesheet>> GetAll();
        Task<Timesheet> GetById(int timesheetId);
        Task<Timesheet> Add(Timesheet timesheet);
        Task Update(Timesheet timesheet);
        Task Delete(int id);
    }
}
