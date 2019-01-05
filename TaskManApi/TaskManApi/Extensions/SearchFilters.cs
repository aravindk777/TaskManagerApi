using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManApi.Extensions
{
    public class SearchFilters
    {
        public string TaskName { get; set; }
        public string ParentTask { get; set; }
        public int? PriorityFrom { get; set; }
        public int? PriorityTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}