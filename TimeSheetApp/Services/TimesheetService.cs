using TimeSheetApp.Models;
using TimeSheetApp.Repository;

namespace TimeSheetApp.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;

        public TimesheetService(ITimesheetRepository timesheetRepository)
        {
            _timesheetRepository = timesheetRepository;
        }

        public Task<Timesheet> Add(Timesheet timesheet)
        {
            return _timesheetRepository.Add(timesheet);
        }

        public Task Delete(int id)
        {
            return _timesheetRepository.Delete(id);
        }

        public Task<IEnumerable<Timesheet>> GetAll()
        {
            return _timesheetRepository.GetAll();
        }

        public async Task Update(Timesheet timesheet)
        {
            await _timesheetRepository.Update(timesheet);
        }

        public Task<Timesheet> GetById(int timesheetId)
        {
            return _timesheetRepository.GetById(timesheetId);
        }
    }
}
