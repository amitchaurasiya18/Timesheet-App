using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        public List<Timesheet>? Timesheets { get; set; }
    }
}
