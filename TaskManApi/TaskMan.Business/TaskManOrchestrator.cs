using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMan.Business.Extensions;
using TaskMan.Business.Model;
using TaskMan.Data;
using TaskMan.Data.Entities;
using TaskMan.Data.Tasks;

namespace TaskMan.Business
{
    public class TaskManOrchestrator : ITaskManLogic
    {
        ITaskRepository taskRepository;

        public TaskManOrchestrator(ITaskRepository _taskRepository)
        {
            this.taskRepository = _taskRepository;
        }

        public int AddNewTask(TaskModel newTask)
        {
            return taskRepository.Add(newTask.ToTaskEntity());

        }

        public int DeleteTask(int taskId, TaskModel delTask = null)
        {
            if (delTask != null)
            {
                if (taskId != delTask.TaskId)
                    throw new InvalidOperationException("TaskId requested is not matching with the Task to be deleted.");
                return taskRepository.Delete(delTask.ToTaskEntity());
            }
            else
            {
                var taskToDelete = taskRepository.Get(taskId);
                return taskRepository.Delete(taskToDelete);
            }
        }

        public bool EndTask(int taskId)
        {
            var task = taskRepository.Get(taskId);
            task.EndDate = DateTime.Now;
            return taskRepository.Update(task) > 0;
        }

        public IEnumerable<TaskModel> GetAllTasks(bool ParentsOnly = false, bool ActiveOnly = false, int Page = 0, int TotalRecords = 0)
        {
            IEnumerable<MyTask> tasks;
            if (ActiveOnly && !ParentsOnly)
                tasks = taskRepository.GetActiveTasks(Page, TotalRecords);
            else if (ParentsOnly && !ActiveOnly)
            {
                if (Page > 0 && TotalRecords > 0)
                    tasks = taskRepository.GetPaginatedAllTasks(Page, TotalRecords);
                else
                    tasks = taskRepository.GetAll()
                            .Where(task => !task.ParentTaskId.HasValue);
            }
            else if (ActiveOnly && ParentsOnly)
                tasks = taskRepository.GetActiveTasks(Page, TotalRecords)
                        .Where(task => !task.ParentTaskId.HasValue);
            else if (Page > 0 && TotalRecords > 0 && !ActiveOnly && !ParentsOnly)
                tasks = taskRepository.GetPaginatedAllTasks(Page, TotalRecords);
            else
                tasks = taskRepository.GetAll();

            return tasks.ToModel();
        }

        public TaskModel GetTask(int taskId, string TaskName = "")
        {
            if (!string.IsNullOrEmpty(TaskName))
                return taskRepository.Search(task => task.TaskName.ToLower().Contains(TaskName.ToLower())).ToModel().FirstOrDefault();
            return taskRepository.Get(taskId).ToModel();
        }

        public int GetTotalTasksCount()
        {
            return taskRepository.GetTotalTasksCount();
        }

        public int UpdateMyTask(int taskId, TaskModel task)
        {
            if (taskId != task.TaskId)
                throw new InvalidOperationException("TaskId requested is not matching with the Task to be updated with.");
            return taskRepository.Update(task.ToTaskEntity());
        }
    }
}