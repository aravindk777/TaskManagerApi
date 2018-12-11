using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMan.Business.Model;
using TaskMan.Data;
using TaskMan.Data.Entities;

namespace TaskMan.Business
{
    public interface ITaskManLogic
    {
        int AddNewTask(TaskModel newTask);
        IEnumerable<TaskModel> GetAllTasks(bool ParentsOnly = false, bool ActiveOnly = false, int Page = 0, int TotalRecords = 0);
        TaskModel GetTask(int taskId, string TaskName = "");
        TaskModel UpdateMyTask(int taskId, TaskModel task);
        int DeleteTask(int taskId, TaskModel delTask = null);
        bool EndTask(int taskId);
    }
}
