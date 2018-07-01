using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TaskManApi.Controllers;
using TaskManApi.Data;

namespace TaskManApi.Tests.Controllers
{
    [TestClass]
    public class TaskControllerTest
    {
        [TestMethod]
        public void GetTask_Test_For_ExistingOne()
        {
            var dbRepo = new Mock<TaskManDb>(MockBehavior.Strict);
            var mockTasks = new List<Models.Task>()
                            {
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
                                new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
                            };
            //dbRepo.Object.Tasks.AddRange(mockTasks);
            var taskCtrler = new TasksController(dbRepo.Object);
            Assert.IsNotNull(value: taskCtrler.GetTasks());
        }

        [TestMethod]
        public void GetAllTasks_To_Test_Get_Method()
        {

        }
    }
}
