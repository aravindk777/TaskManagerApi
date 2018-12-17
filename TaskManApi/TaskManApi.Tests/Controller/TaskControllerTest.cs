using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TaskMan.Business;
using System.Net.Http;
using TaskMan.Business.Model;
using TaskManApi.Controllers;
using System.Web.Http.Results;
using System.Web.Http;

namespace TaskManApi.Tests.Controller
{
    [TestFixture]
    public class TaskControllerTest
    {
        Mock<ITaskManLogic> mockOrchestrator;
        IEnumerable<TaskModel> mockedModelObjects;
        TasksController testControllerObj;
        TaskModel[] testModels = new TaskModel[] { };

        [SetUp]
        public void Init()
        {
            mockOrchestrator = new Mock<ITaskManLogic>(MockBehavior.Strict);
            mockedModelObjects = new List<TaskModel>() {
                new TaskModel(){TaskId = 1, TaskName = "test 1", ParentTaskId = null, StartDate = System.DateTime.Now, Priority = 1},
                new TaskModel(){TaskId = 2, TaskName = "test 2", ParentTaskId = 1, StartDate = System.DateTime.Now, Priority = 10},
                new TaskModel(){TaskId = 3, TaskName = "test 3", ParentTaskId = null, StartDate = System.DateTime.Now.AddMonths(-1), Priority = 2, EndDate = System.DateTime.Now.AddDays(-1)},
                new TaskModel(){TaskId = 4, TaskName = "test 4", ParentTaskId = 2, StartDate = System.DateTime.Now, Priority = 3, EndDate = System.DateTime.Now.AddDays(10)},
                new TaskModel(){TaskId = 5, TaskName = "test 5", ParentTaskId = 0, StartDate = System.DateTime.Now, Priority = 4},
                new TaskModel(){TaskId = 6, TaskName = "test 6", ParentTaskId = 11, StartDate = System.DateTime.Now, Priority = 10, EndDate = System.DateTime.Now.AddDays(30)},
                new TaskModel(){TaskId = 7, TaskName = "test 7", ParentTaskId = 4, StartDate = System.DateTime.Now.AddDays(-5), Priority = 5, EndDate = System.DateTime.Now},
                new TaskModel(){TaskId = 8, TaskName = "test 8", ParentTaskId = 0, StartDate = System.DateTime.Now, Priority = 15},
                new TaskModel(){TaskId = 9, TaskName = "test 9", ParentTaskId = 0, StartDate = System.DateTime.Now, Priority = 20, EndDate = System.DateTime.Now.AddMonths(3)},
                new TaskModel(){TaskId = 10, TaskName = "test 10", ParentTaskId = 1, StartDate = System.DateTime.Now, Priority = 30},
                new TaskModel(){TaskId = 11, TaskName = "test 11", ParentTaskId = 4, StartDate = System.DateTime.Now.AddDays(-10), Priority = 25, EndDate = System.DateTime.Now.AddDays(-1)},
            };

            testControllerObj = new TasksController(mockOrchestrator.Object);

            testModels = new TaskModel[] {
                new TaskModel() { TaskId = 45, TaskName = "Mocking add", Priority = 4 },
                new TaskModel() { TaskId = 8, TaskName = "Mocking existing task", Priority = 5 },
                new TaskModel() { TaskName = "Mocking with no taskId", Priority = 10 },
                new TaskModel() { TaskId = 60, TaskName = "Mocking add" }
            };
        }

        [TestCase(0, 0, 11, TestName = "Get All tasks")]
        [TestCase(1, 5, 5, TestName = "Pagination with 1st page with 5 records per page")]
        [TestCase(3, 4, 3, TestName = "Pagination with 3rd page with 4 records per page")]
        [TestCase(2, 10, 1, TestName = "Pagination with 2nd page with 10 records per page")]
        [Test]
        public void Test_For_GetAll(int mockPageIndex, int mockPageSize, int expectedCount)
        {
            //Arrange
            var dataToCompare = mockedModelObjects;
            if (mockPageIndex > 0 && mockPageSize > 0)
            {
                dataToCompare = dataToCompare.Skip((mockPageIndex - 1) * mockPageSize).Take(mockPageSize);
            }
            mockOrchestrator.Setup(t => t.GetAllTasks(It.IsAny<bool>(), It.IsAny<bool>(), mockPageIndex, mockPageSize)).Returns(dataToCompare);

            //Act
            var result = testControllerObj.GetAllTasks(mockPageIndex, mockPageSize);
            IEnumerable<TaskModel> actualData = ((OkNegotiatedContentResult<IEnumerable<TaskModel>>)result).Content;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(actualData);
            Assert.That(actualData.Count(), Is.EqualTo(expectedCount));

            //Final Assertion for all Test cases
            mockOrchestrator.VerifyAll();

        }

        [TestCase(true, 8, TestName = "Active tasks")]
        [Test]
        public void Test_For_GetActiveTasks(bool mockActive, int expectedCount)
        {
            //Arrange
            var dataToCompare = mockedModelObjects;
            if (mockActive)
                dataToCompare = dataToCompare.Where(item => (!item.EndDate.HasValue || (item.EndDate.HasValue && item.EndDate.Value.CompareTo(System.DateTime.Now) > 0)));
            mockOrchestrator.Setup(t => t.GetAllTasks(It.IsAny<bool>(), mockActive, It.IsAny<int>(), It.IsAny<int>())).Returns(dataToCompare);

            //Act
            var result = testControllerObj.GetActiveTasks();
            var actualData = ((OkNegotiatedContentResult<IEnumerable<TaskModel>>)result).Content;
            int actualCount = actualData.Count();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IHttpActionResult>(result);
            Assert.IsNotNull(actualData);
            Assert.That(actualCount, Is.EqualTo(expectedCount));
            mockOrchestrator.VerifyAll();
        }

        [TestCase(true, 5, TestName = "Parent tasks only")]
        [Test]
        public void Test_For_ParentsOnly(bool mockParentsOnly, int expectedCount)
        {

            var dataToCompare = mockedModelObjects;
            if (mockParentsOnly)
                dataToCompare = dataToCompare.Where(item => (!item.ParentTaskId.HasValue || (item.ParentTaskId.HasValue && item.ParentTaskId.Value == 0)));
            mockOrchestrator.Setup(t => t.GetAllTasks(mockParentsOnly, It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).Returns(dataToCompare);

            //Act
            var result = testControllerObj.GetParentTasks();
            var actualData = ((OkNegotiatedContentResult<IEnumerable<TaskModel>>)result).Content;
            int actualCount = actualData.Count();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IHttpActionResult>(result);
            Assert.IsNotNull(actualData);
            Assert.That(actualCount, Is.EqualTo(expectedCount));
            mockOrchestrator.VerifyAll();
        }

        [TestCase(5, typeof(OkNegotiatedContentResult<TaskModel>), TestName = "Get an existing Task - Id 5")]
        [TestCase(30, typeof(NotFoundResult), TestName = "Get non existing task id")]
        [Test]
        public void Test_For_GetTaskById(int TaskId, System.Type expectedType)
        {
            // Arrange
            var dataToCompare = mockedModelObjects;
            mockOrchestrator.Setup(t => t.GetTask(TaskId, It.IsAny<string>())).Returns(dataToCompare.FirstOrDefault(task => task.TaskId == TaskId));

            //Act
            var result = testControllerObj.Get(TaskId);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IHttpActionResult>(result);
            Assert.AreEqual(expectedType, result.GetType());
            mockOrchestrator.VerifyAll();
        }

        [TestCase(0, typeof(CreatedNegotiatedContentResult<TaskModel>), TestName = "Create new task - test1")]
        [TestCase(1, typeof(CreatedNegotiatedContentResult<TaskModel>), TestName = "Create new task - test2")]
        [Test]
        public void Test_For_PostNewTask(int modelArrPos, System.Type expectedType)
        {
            // Arrange
            TaskModel newTaskMock = testModels[modelArrPos];
            mockOrchestrator.Setup(t => t.AddNewTask(newTaskMock)).Returns(1);

            // Act
            var result = testControllerObj.Post(newTaskMock);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedType, result.GetType());
            mockOrchestrator.VerifyAll();
        }

        [TestCase(1, 1, typeof(OkNegotiatedContentResult<bool>), TestName = "Updating task - success test")]
        [TestCase(2, 0, typeof(BadRequestErrorMessageResult), TestName = "Updating task - Not Found test")]
        [TestCase(3, -1, typeof(ExceptionResult), TestName = "Updating task - Exception")]
        [Test]
        public void Test_For_UpdateTask(int modelArrPos, int expectedUpdateResult, System.Type expectedType)
        {
            // Arrange
            TaskModel testTaskForUpdate = testModels[modelArrPos];
            testTaskForUpdate.TaskName = "updated task name";
            if (expectedUpdateResult >= 0)
                mockOrchestrator.Setup(t => t.UpdateMyTask(testTaskForUpdate.TaskId, testTaskForUpdate)).Returns(expectedUpdateResult);
            else
                mockOrchestrator.Setup(t => t.UpdateMyTask(testTaskForUpdate.TaskId, It.IsAny<TaskModel>())).Throws<System.Exception>();

            // Act
            var result = testControllerObj.Put(testTaskForUpdate.TaskId, testTaskForUpdate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedType, result.GetType());
            mockOrchestrator.VerifyAll();
        }

        [TestCase(1, 1, typeof(OkNegotiatedContentResult<bool>), TestName = "Delete task - success test")]
        [TestCase(2, 0, typeof(NotFoundResult), TestName = "Delete task - Not Found test")]
        [TestCase(5, -1, typeof(ExceptionResult), TestName = "Delete task - Exception")]
        [Test]
        public void Test_For_DeleteTask(int taskIdToDelete, int expectedUpdateResult, System.Type expectedType)
        {
            // Arrange
            TaskModel testTaskForDelete = mockedModelObjects.FirstOrDefault(task => task.TaskId == taskIdToDelete);
            if (expectedUpdateResult >= 0)
                mockOrchestrator.Setup(t => t.DeleteTask(taskIdToDelete, It.IsAny<TaskModel>())).Returns(expectedUpdateResult);
            else
                mockOrchestrator.Setup(t => t.DeleteTask(taskIdToDelete, It.IsAny<TaskModel>())).Throws<System.Exception>();

            // Act
            var result = testControllerObj.Delete(taskIdToDelete);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedType, result.GetType());
            mockOrchestrator.VerifyAll();
        }

        [TestCase(1, true, typeof(OkNegotiatedContentResult<bool>), TestName = "End task - End is true")]
        [TestCase(2, false, typeof(OkNegotiatedContentResult<bool>), TestName = "End task - End is false")]
        [Test]
        public void Test_For_EndTask(int TaskIdToMarkAsEnded, bool expectedUpdateResult, System.Type expectedType)
        {
            // Arrange
            mockOrchestrator.Setup(t => t.EndTask(TaskIdToMarkAsEnded)).Returns(expectedUpdateResult);

            // Act
            var result = testControllerObj.EndTask(TaskIdToMarkAsEnded);
            var status = ((OkNegotiatedContentResult<bool>)result).Content;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedType, result.GetType());
            Assert.AreEqual(expectedUpdateResult, status);
            mockOrchestrator.VerifyAll();
        }
    }
}
