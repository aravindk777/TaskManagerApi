using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManApi.Models
{
    public class Task
    {
        [Key]
        public Guid TaskId { get; set; }
        public Guid? ParentTaskId { get; set; }
        [Required]
        public string TaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(0,30, ErrorMessage ="Value doesnt fall within acceptable Range (0 through 30)")]
        public int Priority { get; set; }
    }
}