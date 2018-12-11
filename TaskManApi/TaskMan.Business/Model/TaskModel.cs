using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMan.Business.Model
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public int? ParentTaskId { get; set; }
        [Required]
        public string TaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        [RegularExpression("[1-30]{1,}")]
        public int Priority { get; set; }
        public string Status { get; set; }
    }
}
