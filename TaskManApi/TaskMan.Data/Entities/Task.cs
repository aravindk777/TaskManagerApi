namespace TaskMan.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Table name renamed here as MyTask to negate the conflict with System.Threading.Task. 
    /// I could have used the full naming convention format including Namespace but i wonder signature exposed through api may conflict or confusing
    /// </summary>
    [Table("Task")]
    public partial class MyTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public int? ParentTaskId { get; set; }
        [Required]
        public string TaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public virtual MyTask ParentTask { get; set; }
    }
}
