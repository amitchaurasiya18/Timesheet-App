using TimeSheetApp.Models;

namespace TimeSheetApp.Repository
{
    public interface ITimesheetRepository
    {
        Task<IEnumerable<Timesheet>> GetAll();
        Task<Timesheet> GetById(int timesheetId);
        Task<Timesheet> Add(Timesheet timesheet);
        Task Update(Timesheet timesheet);
        Task Delete(int id);
    }
}
