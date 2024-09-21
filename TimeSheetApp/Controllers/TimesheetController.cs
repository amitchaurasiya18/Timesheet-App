using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeSheetApp.Models;
using TimeSheetApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimeSheetApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetService _timesheetService;

        public TimesheetController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var timesheets = await _timesheetService.GetAll();
            return Ok(timesheets);
        }

        [HttpGet("{timesheetId}")]
        public async Task<IActionResult> GetByTimesheetId(int timesheetId)
        {
            var timesheet = await _timesheetService.GetById(timesheetId);
            if (timesheet == null)
            {
                return NotFound();
            }
            return Ok(timesheet);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Timesheet timesheet)
        {
            await _timesheetService.Add(timesheet);
            return Ok(timesheet);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Timesheet updatedTimesheet)
        {
            var existingTimesheet = await _timesheetService.GetById(id);
            if (existingTimesheet == null)
            {
                return NotFound();
            }

            existingTimesheet.Date = updatedTimesheet.Date;
            existingTimesheet.IsOnLeave = updatedTimesheet.IsOnLeave;
            existingTimesheet.Activities = updatedTimesheet.Activities;

            await _timesheetService.Update(existingTimesheet);

            return Ok(existingTimesheet);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _timesheetService.Delete(id);
            return NoContent();
        }
    }
}
