using System;
using System.ComponentModel.DataAnnotations;

namespace TaskMan.Business.Model
{
    public class TaskModel
    {
        public int TaskId { get; set; }

        public int? ParentTaskId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Task Name is required")]
        public string TaskName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Priority is required", AllowEmptyStrings = false)]
        [Range(1,30, ErrorMessage = "Please provide values between 1 and 30")]
        public int Priority { get; set; }

        public string Status { get; set; }
        public string ParentTask { get; internal set; }
    }
}
