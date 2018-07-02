using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManApi.Tests.Data
{
    public class TestDataForTaskModel
    {
        public static IDbSet<Models.Task> GetTestDataForTasks(List<Models.Task> mockTasks = null)
        {
            if (mockTasks == null || mockTasks.Count == 0)
                mockTasks = new List<Models.Task>()
                            {
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
                            };

            var testDbSet = new Mock<DbSet<Models.Task>>();
            var queryableTestTaskData = mockTasks.AsQueryable();
            testDbSet.As<IQueryable<Models.Task>>().Setup(prov => prov.Provider).Returns(queryableTestTaskData.Provider);
            testDbSet.As<IQueryable<Models.Task>>().Setup(prov => prov.Expression).Returns(queryableTestTaskData.Expression);
            testDbSet.As<IQueryable<Models.Task>>().Setup(prov => prov.ElementType).Returns(queryableTestTaskData.ElementType);
            testDbSet.As<IQueryable<Models.Task>>().Setup(prov => prov.GetEnumerator()).Returns(() => queryableTestTaskData.GetEnumerator());

            return testDbSet.Object;
        }
    }
}
