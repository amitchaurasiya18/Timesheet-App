using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        public string Project { get; set; }
        public string SubProject { get; set; }
        public string Batch { get; set; }
        public int HoursNeeded { get; set; }
        public string ActivityDescription { get; set; }
        public int TimesheetId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public Timesheet? Timesheet { get; set; }   
    }
}
