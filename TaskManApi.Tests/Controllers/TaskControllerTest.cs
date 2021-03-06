﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TaskManApi.Controllers;
using TaskManApi.Data;
using TaskManApi.Tests.Data;
using TaskManApi.Tests.TestDataModels;

namespace TaskManApi.Tests.Controllers
{
    [TestClass]
    public class TaskControllerTest
    {
        TasksController ctrler;
        //[TestMethod]
        //public void GetTask_Test_For_ExistingOne()
        //{
        //    var dbRepo = new Mock<TaskManDb>(MockBehavior.Strict);
        //    var mockTasks = new List<Models.Task>()
        //                    {
        //                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
        //                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
        //                    };
        //    //dbRepo.Object.Tasks.AddRange(mockTasks);
        //    var taskCtrler = new TasksController(dbRepo.Object);
        //    Assert.IsNotNull(value: taskCtrler.GetTasks());
        //}

        [TestMethod]
        public void GetAllTasks_To_Test_Get_Method()
        {
            var listOfTestTasks = new List<Models.Task>()
                                    {
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-3", Priority = 10},
                                    };
            var testContext = new TestTaskManDb(listOfTestTasks);
            var ctrler = new TasksController(testContext);
            var result = ctrler.GetTasks();

            var listOfResult = result.ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(listOfTestTasks.Count, listOfResult.Count);
            Assert.AreEqual(listOfTestTasks[0], listOfResult[0]);
        }

        [TestMethod]
        public void GetAllTasks_When_No_DataExists()
        {
            var testContext = new TestTaskManDb();
            var ctrler = new TasksController(testContext);
            var result = ctrler.GetTasks();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count().Equals(0));
            Assert.IsInstanceOfType(result, typeof(IQueryable<Models.Task>));
        }

        [TestMethod]
        public void GetTask_With_TaskId()
        {
            var testTasksForThis = new List<Models.Task>()
                                    {
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-1", Priority = 5 },
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-2", Priority = 10},
                                        new Models.Task() { TaskId = Guid.NewGuid(), TaskName = "test-task-3", Priority = 10},
                                    };
            var testContext = new TestTaskManDb(testTasksForThis);

            var ctrler = new TasksController(testContext);
            var testTaskData = testTasksForThis.FirstOrDefault();
            var result = ctrler.GetTask(testTaskData.TaskId) as OkNegotiatedContentResult<Models.Task>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(testTaskData, result.Content);
        }

        [TestMethod]
        public void GetTask_Test_For_Invalid_TaskId()
        {
            var testCtx = new TestTaskManDb();
            ctrler = new TasksController(testCtx);
            var result = ctrler.GetTask(Guid.NewGuid()) as OkNegotiatedContentResult<Models.Task>;

            Assert.IsNull(result.Content);
        }

        [TestMethod]
        public void Test_For_PostNewTask()
        {
            var testCxt = new TestTaskManDb();
            ctrler = new TasksController(testCxt);

            var newTestTask = new Models.Task() { TaskName = "Testing for posting a new task", Priority = 5, StartDate = DateTime.Today };
            var result = ctrler.PostTask(newTestTask) as CreatedAtRouteNegotiatedContentResult<Models.Task>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(newTestTask.TaskName, result.Content.TaskName);
        }

        [TestMethod]
        public void Test_For_DeleteExistingTask()
        {
            var testCxt = new TestTaskManDb();
            testCxt.Tasks = TestDataForTaskModel.GetTestDataForTasks();
            ctrler = new TasksController(testCxt);
            var taskToDelete = testCxt.Tasks.FirstOrDefault();
            var result = ctrler.DeleteTask(taskToDelete.TaskId) as OkNegotiatedContentResult<Models.Task>;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.IsNull(testCxt.Tasks.Find(taskToDelete.TaskId));
        }
    }
}
