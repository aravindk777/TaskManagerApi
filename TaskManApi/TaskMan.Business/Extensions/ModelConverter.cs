using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMan.Business.Model;
using TaskMan.Data.Entities;

namespace TaskMan.Business.Extensions
{
    public static class ModelConverter
    {
        public static IEnumerable<TaskModel> ToModel(this IEnumerable<MyTask> entities)
        {
            return entities.Select(ent => 
                    new TaskModel
                    {
                        TaskId = ent.TaskId, TaskName = ent.TaskName, ParentTaskId = ent.ParentTaskId,
                        StartDate = ent.StartDate, EndDate = ent.EndDate, Priority = ent.Priority, Status = ent.Status
                    });
        }

        public static TaskModel ToModel(this MyTask entity)
        {
            return new TaskModel
            {
                TaskId = entity.TaskId,
                TaskName = entity.TaskName,
                ParentTaskId = entity.ParentTaskId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Priority = entity.Priority,
                Status = entity.Status
            };
        }

        public static MyTask ToTaskEntity(this TaskModel model)
        {
            return new MyTask
            {
                TaskId = model.TaskId,
                TaskName = model.TaskName,
                ParentTaskId = model.ParentTaskId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Priority = model.Priority,
                Status = model.Status
            };
        }
    }
}
