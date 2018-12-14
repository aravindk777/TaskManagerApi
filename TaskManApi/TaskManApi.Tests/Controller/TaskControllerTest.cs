using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TaskMan.Business;
using System.Net.Http;
using TaskMan.Business.Model;
using TaskManApi.Controllers;

namespace TaskManApi.Tests.Controller
{
    [TestFixture]
    public class TaskControllerTest
    {
        Mock<ITaskManLogic> mockOrchestrator;
        IEnumerable<TaskModel> mockedModelObjects;
        TasksController testControllerObj;

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
        }

        [TestCase(0,0, 11)]
        [TestCase(1,5, 5)]
        [TestCase(3, 4, 3)]
        [TestCase(2, 10, 1)]
        [Test]
        public void GetAll_FromMockedBusinessLayer_Test(int mockPageIndex, int mockPageSize, int expectedCount)
        {
            //Arrange
            var dataToCompare = mockedModelObjects;
            if (mockPageIndex > 0 && mockPageSize > 0)
            {
                dataToCompare = dataToCompare.Skip((mockPageIndex-1)* mockPageSize).Take(mockPageSize);
            }
            mockOrchestrator.Setup(t => t.GetAllTasks(It.IsAny<bool>(), It.IsAny<bool>(), mockPageIndex, mockPageSize)).Returns(dataToCompare);

            //Act
            var result = testControllerObj.GetAllTasks(mockPageIndex, mockPageSize);
            IEnumerable<TaskModel> actualData;
            var status = result.TryGetContentValue(out actualData);

            //Assert
            Assert.IsNotNull(result);
            //Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.IsNotEmpty(actualData);
            Assert.That(actualData.Count(), Is.EqualTo(expectedCount));

            //Final Assertion for all Test cases
            mockOrchestrator.VerifyAll();
            
        }

        //[TestCase(false, false, 0, 0, 11)]
        //[TestCase(false, false, 1, 5, 5)]
        //[TestCase(true, false, 0, 0, 5)]
        //[TestCase(false, true, 0, 0, 8)]
        //[Test]
        //public void GetAll_FromMockedBusinessLayer_Test(bool mockParentsOnly, bool mockActive, int mockPageIndex, int mockPageSize, int expectedCount)
        //{
        //    //Arrange
        //    var dataToCompare = mockedModelObjects;
        //    if (mockParentsOnly)
        //        dataToCompare = dataToCompare.Where(item => (!item.ParentTaskId.HasValue || (item.ParentTaskId.HasValue && item.ParentTaskId.Value == 0)));
        //    if (mockActive)
        //        dataToCompare = dataToCompare.Where(item => (!item.EndDate.HasValue || (item.EndDate.HasValue && item.EndDate.Value.CompareTo(System.DateTime.Now) > 0)));
        //    if (mockPageIndex > 0)
        //        dataToCompare = dataToCompare.Skip(mockPageIndex - 1);
        //    if (mockPageSize > 0)
        //        dataToCompare = dataToCompare.Take(mockPageSize);
        //    mockOrchestrator.Setup(t => t.GetAllTasks(mockParentsOnly, mockActive, mockPageIndex, mockPageSize)).Returns(dataToCompare);

        //    //Act
        //    var result = testControllerObj.GetAll(mockPageIndex, mockPageSize);
        //    var actualData = ((OkNegotiatedContentResult<IEnumerable<TaskModel>>)result).Content;

        //    //Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<IHttpActionResult>(result);
        //    Assert.IsNotEmpty(actualData);
        //    Assert.That(actualData.Count(), Is.EqualTo(expectedCount));

        //    mockOrchestrator.VerifyAll();

        //}
    }
}
