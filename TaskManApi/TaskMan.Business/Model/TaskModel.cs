using System;
using System.ComponentModel.DataAnnotations;

namespace TaskMan.Business.Model
{
    public class TaskModel
    {
        [Display(Order =1)]
        public int TaskId { get; set; }
        
        public int? ParentTaskId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Task Name is required")]
        [Display(Order = 2)]
        public string TaskName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        [Display(Order = 4)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date")]
        [Display(Order = 5)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Priority is required", AllowEmptyStrings = false)]
        [Range(1,30, ErrorMessage = "Please provide values between 1 and 30")]
        [Display(Order = 3)]
        public int Priority { get; set; }

        public string Status { get; set; }

        [Display(Order = 6)]
        public string ParentTask { get; internal set; }
    }
}
