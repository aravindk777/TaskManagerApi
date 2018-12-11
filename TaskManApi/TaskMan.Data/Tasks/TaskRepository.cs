using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskMan.Data.Entities;
using TaskMan.Data.Repository;

namespace TaskMan.Data.Tasks
{
    public class TaskRepository : Repository<MyTask>, ITaskRepository
    {
        public TaskManagerContext TaskDbContext { get { return Context as TaskManagerContext; } }
        public TaskRepository(TaskManagerContext context) : base(context)
        {
        }

        public IEnumerable<MyTask> GetActiveTasks(int pageIndex = 0, int pageSize = 0)
        {
            if (pageIndex > 0 && pageSize > 0)
            {
                return GetAll()
                .Where(task => !task.EndDate.HasValue || (task.EndDate.HasValue && DateTime.Compare(task.EndDate.Value, DateTime.Today) >= 1))
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
            }
            else
            {
                return GetAll()
                .Where(task => !task.EndDate.HasValue || (task.EndDate.HasValue && DateTime.Compare(task.EndDate.Value, DateTime.Today) >= 1));
            }
        }

        public IEnumerable<MyTask> GetPaginatedAllTasks(int pageIndex = 0, int pageSize = -1)
        {
            return GetAll()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
        }
    }
}
