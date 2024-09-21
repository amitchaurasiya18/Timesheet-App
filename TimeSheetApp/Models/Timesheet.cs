using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class Timesheet
    {
        [Key]
        public int TimesheetId { get; set; }
        public DateTime Date { get; set; }
        public bool IsOnLeave { get; set; }
        public List<Activity> Activities { get; set; }   
        
        public int UserId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public User User { get; set; }
    }
}
