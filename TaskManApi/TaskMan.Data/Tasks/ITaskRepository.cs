using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMan.Data.Entities;
using TaskMan.Data.Repository;

namespace TaskMan.Data.Tasks
{
    public interface ITaskRepository : IRepository<MyTask>
    {
        IEnumerable<MyTask> GetPaginatedAllTasks(int pageIndex, int pageSize);
        IEnumerable<MyTask> GetActiveTasks(int pageIndex = 0, int pageSize = 0);
    }
}
