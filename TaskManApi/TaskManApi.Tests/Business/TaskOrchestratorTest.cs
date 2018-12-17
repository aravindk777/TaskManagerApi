using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskMan.Business;
using TaskMan.Data.Entities;
using TaskMan.Data.Tasks;

namespace TaskManApi.Tests.Business
{
    [TestFixture]
    public class TaskOrchestratorTest
    {
        // Test instances to be used within this test fixture
        Mock<ITaskRepository> mockTaskRepo;
        ITaskManLogic mockTaskOrchestrator;
        IEnumerable<MyTask> mockedDataSets;
        TaskMan.Business.Model.TaskModel[] testDataForAdd = new TaskMan.Business.Model.TaskModel[] { };

        [SetUp]
        public void InitTestAndSetup()
        {
            mockTaskRepo = new Mock<ITaskRepository>(MockBehavior.Strict);
            mockTaskOrchestrator = new TaskManOrchestrator(mockTaskRepo.Object);

            mockedDataSets = new List<MyTask>() {
                new MyTask(){TaskId = 1, TaskName = "test 1", StartDate = System.DateTime.Now, Priority = 1},
                new MyTask(){TaskId = 2, TaskName = "test 2", StartDate = System.DateTime.Now, Priority = 10},
                new MyTask(){TaskId = 3, TaskName = "test 3", StartDate = System.DateTime.Now.AddMonths(-1), Priority = 2, EndDate = System.DateTime.Now.AddDays(-1)},
                new MyTask(){TaskId = 4, TaskName = "test 4", StartDate = System.DateTime.Now, Priority = 3, EndDate = System.DateTime.Now.AddDays(10)},
                new MyTask(){TaskId = 5, TaskName = "test 5", StartDate = System.DateTime.Now, Priority = 4},
                new MyTask(){TaskId = 6, TaskName = "test 6", StartDate = System.DateTime.Now, Priority = 10, EndDate = System.DateTime.Now.AddDays(30)},
                new MyTask(){TaskId = 7, TaskName = "test 7", StartDate = System.DateTime.Now.AddDays(-5), Priority = 5, EndDate = System.DateTime.Now},
                new MyTask(){TaskId = 8, TaskName = "test 8", StartDate = System.DateTime.Now, Priority = 15},
                new MyTask(){TaskId = 9, TaskName = "test 9", StartDate = System.DateTime.Now, Priority = 20, EndDate = System.DateTime.Now.AddMonths(3)},
                new MyTask(){TaskId = 10, TaskName = "test 10", StartDate = System.DateTime.Now, Priority = 30},
                new MyTask(){TaskId = 11, TaskName = "test 11", StartDate = System.DateTime.Now.AddDays(-10), Priority = 25, EndDate = System.DateTime.Now.AddDays(-1)},
            };

            //setting up parent tasks in Entity object level
            mockedDataSets.ElementAt(1).ParentTask = mockedDataSets.ElementAt(9).ParentTask = mockedDataSets.ElementAt(0);
            mockedDataSets.ElementAt(1).ParentTaskId = 1;
            mockedDataSets.ElementAt(9).ParentTaskId = 1;

            mockedDataSets.ElementAt(3).ParentTask = mockedDataSets.ElementAt(1);
            mockedDataSets.ElementAt(3).ParentTaskId = 2;

            mockedDataSets.ElementAt(4).ParentTask = mockedDataSets.ElementAt(4);
            mockedDataSets.ElementAt(4).ParentTaskId = 5;

            mockedDataSets.ElementAt(5).ParentTask = mockedDataSets.ElementAt(10);
            mockedDataSets.ElementAt(5).ParentTaskId = 11;

            mockedDataSets.ElementAt(6).ParentTask = mockedDataSets.ElementAt(10).ParentTask = mockedDataSets.ElementAt(7).ParentTask = mockedDataSets.ElementAt(3);
            mockedDataSets.ElementAt(6).ParentTaskId = 4;
            mockedDataSets.ElementAt(10).ParentTaskId = 4;
            mockedDataSets.ElementAt(7).ParentTaskId = 4;

            testDataForAdd = new TaskMan.Business.Model.TaskModel[] {
                new TaskMan.Business.Model.TaskModel(){TaskName = "New Task", StartDate = System.DateTime.Now},
                new TaskMan.Business.Model.TaskModel(){TaskName = "New Task with no other fields"},
                new TaskMan.Business.Model.TaskModel(){TaskName = "", Status ="Test for empty"},
            };
        }

        [TestCase(false, false, 0, 0, ExpectedResult = 11, TestName = "All Tasks", Description = "All Tasks")]          // all tasks
        [TestCase(true, false, 0, 0, ExpectedResult = 3, TestName = "Parents Tasks only", Description = "Parent tasks only")]            // Parents only tasks - note: ParenttaskId = 0 is also considered as having a parent Task with Id=0
        [TestCase(false, true, 0, 0, ExpectedResult = 8, TestName = "Active tasks only", Description = "Active tasks only")]             // Active tasks only
        [TestCase(true, true, 0, 0, ExpectedResult = 2, TestName = "Active Parent tasks only", Description = "Active Parent tasks only")]             // Active Parent tasks only
        [TestCase(false, false, 4, 3, ExpectedResult = 2, TestName = "Pagination", Description = "Pagination")]           // pagination
        [TestCase(false, true, 2, 3, ExpectedResult = 3, TestName = "Pagination for active tasks only", Description = "Pagination for active tasks only")]           // pagination for active tasks
        [Test]
        public void Test_For_Orchestrator_GetAllTasks(bool parentsOnly, bool activeOnly, int pageIndex, int pageSize, int expectedResultCount)
        {
            // Arrange
            mockTaskRepo.Setup(tRepo => tRepo.GetAll()).Returns(mockedDataSets);
            mockTaskRepo.Setup(repo => repo.GetActiveTasks(pageIndex, pageSize)).Returns(mockedDataSets.Where(t => !t.EndDate.HasValue || t.EndDate.Value > System.DateTime.Now));
            mockTaskRepo.Setup(repo => repo.GetPaginatedAllTasks(pageIndex, pageSize)).Returns(mockedDataSets.Skip((pageIndex - 1) * pageSize).Take(pageSize));

            // Act
            var result = mockTaskOrchestrator.GetAllTasks(parentsOnly, activeOnly, pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count());
        }

        [TestCase(0, ExpectedResult = 1, TestName = "Add new task")]
        [TestCase(1, ExpectedResult = 1, TestName = "Add new task with no other fields")]
        [TestCase(2, ExpectedResult = 1, TestName = "Add new task with no task name")]
        [Test]
        public void Test_For_Orchestrator_AddNewTask(int arryPosForMockAdd, int expectedResult)
        {
            // Arrange
            mockTaskRepo.Setup(repo => repo.Add(It.IsAny<MyTask>())).Returns(1);

            // Act
            var result = mockTaskOrchestrator.AddNewTask(testDataForAdd[arryPosForMockAdd]);

            // Assert
            Assert.IsTrue(result == expectedResult);
        }

        [TestCase(1, null, "test 1", null, Category = "Get Task", TestName = "Get a parent task")]
        [TestCase(2, null, "test 2", 1, Category = "Get Task", TestName = "Get a task with parent task associated")]
        [TestCase(5, null, "test 5", 5, Category = "Get Task", TestName = "Get a task associated self as parent task")]
        [TestCase(11, "test 11", "test 11", 4, Category = "Get Task", TestName = "Get a task by task name")]
        [Test(Description = "Test for Get Task")]
        public void Test_For_Orchestrator_GetTask(int mockTaskId, string inputTaskName, string expectedTaskName, int? expectedParentTaskId)
        {
            // Arrange
            mockTaskRepo.Setup(repo => repo.Get(mockTaskId)).Returns(mockedDataSets.FirstOrDefault(t => t.TaskId.Equals(mockTaskId)));
            mockTaskRepo.Setup(repo => repo.Search(It.IsAny<System.Linq.Expressions.Expression<Func<MyTask, bool>>>())).Returns(mockedDataSets.Where(t => t.TaskName.Equals(inputTaskName)));

            // Act
            TaskMan.Business.Model.TaskModel result;
            if (!string.IsNullOrEmpty(inputTaskName))
                result = mockTaskOrchestrator.GetTask(0, inputTaskName);
            else
                result = mockTaskOrchestrator.GetTask(mockTaskId);

            // Assert
            Assert.AreEqual(expectedTaskName, result.TaskName);
            Assert.AreEqual(expectedParentTaskId, result.ParentTaskId);
        }

        [TestCase(1)]
        [Test]
        public void Test_For_Orchestrator_Update(int mockTaskid)
        {
            MyTask taskToUpdate = mockedDataSets.FirstOrDefault(t => t.TaskId == mockTaskid);
            mockTaskRepo.Setup(repo => repo.Update(It.IsAny<MyTask>()))
                        .Returns(1);

            TaskMan.Business.Model.TaskModel updatingModel = testDataForAdd[0];
            updatingModel.TaskId = mockTaskid;
            var result = mockTaskOrchestrator.UpdateMyTask(mockTaskid, updatingModel);

            // Assert
            Assert.AreEqual(1, result);
            //Assert.AreEqual(mockTaskid, result.TaskId);
        }
    }
}
