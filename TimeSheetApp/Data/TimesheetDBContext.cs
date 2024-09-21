using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Models;

namespace TimeSheetApp.Data
{
    public class TimesheetDBContext : DbContext
    {
        public TimesheetDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Activity> Activities { get; set; }
    }
}
