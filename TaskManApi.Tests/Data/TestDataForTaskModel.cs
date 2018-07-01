using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManApi.Tests.Data
{
    public static class TestDataForTaskModel
    {
        public List<Models.Task> GetTestDataForTasks()
        {
            var mockTasks = new List<Models.Task>()
                            {
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
                            };

            return mockTasks;
        }
    }
}
