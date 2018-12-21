/* 
 * Class -for Task Controller
 * Created by: Aravind Kothandaraman (aravind.pk@aol.in)
 */ 

using System;
using System.Web.Http;
using TaskMan.Business;
using TaskMan.Business.Model;

namespace TaskManApi.Controllers
{
    /// <summary>
    /// Task Controller class holding all the api routes for handling operations on TaskManager
    /// </summary>
    //[RoutePrefix("api")]
    public class TasksController : ApiController
    {
        ITaskManLogic taskOrchestrator;

        /// <summary>
        /// DI constructor
        /// </summary>
        /// <param name="taskOrchestrator"></param>
        public TasksController(ITaskManLogic taskOrchestrator)
        {
            this.taskOrchestrator = taskOrchestrator;
        }

        // GET: api/Tasks
        /// <summary>
        /// Get all the Tasks, either paginated or complete.
        /// If given the PageIndex and PageSize, it would yield paginated results
        /// </summary>
        /// <param name="pageIndex">Non-zero based Page Index. Leave it empty or 0 if no pagination is needed</param>
        /// <param name="pageSize">Non-zero based Page Size. Leave it empty or 0 if no pagination is needed</param>
        /// <returns>Collection of Tasks</returns>
        [HttpGet]
        //[Route(Name = "GetAllTasks")]
        //[ActionName("GetAllTasks")]
        public IHttpActionResult GetAllTasks(int pageIndex, int pageSize)
        {
            try
            {
                var data = taskOrchestrator.GetAllTasks(Page: pageIndex, TotalRecords: pageSize);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Returns all active tasks that hasnt ended yet
        /// </summary>
        /// <returns>List of all active task</returns>
        //[HttpGet]
        [Route("api/Tasks/Active")]
        [ActionName("Active")]
        public IHttpActionResult GetActiveTasks()
        {
            IHttpActionResult result = Ok();
            try
            {
                var data = taskOrchestrator.GetAllTasks(ActiveOnly: true);
                result = Ok(data);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        //GET: tasks/
        //[HttpGet]
        /// <summary>
        /// Get the Parents only tasks. 
        /// </summary>
        /// <returns>List of tasks</returns>
        [Route("api/Tasks/Parent")]
        [ActionName("Parent")]
        public IHttpActionResult GetParentTasks()
        {
            IHttpActionResult result = Ok();
            try
            {
                var data = taskOrchestrator.GetAllTasks(ParentsOnly: true);
                result = Ok(data);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // GET: Tasks/5
        /// <summary>
        /// Get a task object using the Task Id
        /// </summary>
        /// <param name="TaskId">Task Identifier to loate the data</param>
        /// <returns>TaskModel object</returns>
        [HttpGet]
        //[ActionName("GetTask")]
        public IHttpActionResult Get(int TaskId)
        {
            IHttpActionResult result = Ok();
            if (ModelState.IsValid)
                try
                {
                    var data = taskOrchestrator.GetTask(TaskId);
                    if (data == null)
                        result = NotFound();
                    else
                        result = Ok(data);
                }
                catch (Exception ex)
                {
                    result = InternalServerError(ex);
                }
            else
                result = BadRequest(ModelState);

            return result;
        }

        // POST: Tasks
        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="newTask">Task information</param>
        /// <returns>New task Url</returns>
        [HttpPost]
        public IHttpActionResult Post([FromBody]TaskModel newTask)
        {
            if (ModelState.IsValid)
                try
                {
                    taskOrchestrator.AddNewTask(newTask);
                    return Created(new Uri(string.Join("/", Request != null ? Request.RequestUri.ToString() : "http://taskmanapi.local", newTask.TaskId)), newTask);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            else
                return BadRequest(ModelState);
        }

        // PUT: Tasks/5
        /// <summary>
        /// Updates a task for various attributes
        /// </summary>
        /// <param name="taskId">TaskId to locate the task for update</param>
        /// <param name="value">Task data to be updated</param>
        /// <returns>boolean status upon updating</returns>
        [HttpPut]
        public IHttpActionResult Put(int taskId, [FromBody]TaskModel value)
        {
            if (ModelState.IsValid)
                try
                {
                    var updatedData = taskOrchestrator.UpdateMyTask(taskId, value);
                    if (updatedData == 0)
                        return BadRequest("Update failed - Either the data doesnt exists or invalid request to update");
                    else
                        return Ok(true);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(InvalidOperationException))
                        return BadRequest(ex.Message);
                    return InternalServerError(ex);
                }
            else
                return BadRequest(ModelState);
        }

        // DELETE: Tasks/5
        /// <summary>
        /// Deletes a task by TaskId
        /// </summary>
        /// <param name="taskId">Unique identifier to identify a task in the Repository and delete.</param>
        /// <remarks>note - this is a hard Delete. If you are looking for soft delete or ending a task, please use the EndTask route.</remarks>
        /// <returns>boolean status upon deletion process.</returns>
        [HttpDelete]
        public IHttpActionResult Delete(int taskId)
        {
            try
            {
                var status = taskOrchestrator.DeleteTask(taskId);
                if (status <= 0)
                    return NotFound();
                else
                    return Ok(true);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException))
                    return BadRequest(ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// End a task by marking the end date as current date
        /// </summary>
        /// <param name="taskId">Task Identifier to end</param>
        /// <returns>status as success or failure on ending the task</returns>
        [HttpPost]
        public IHttpActionResult EndTask(int taskId)
        {
            try
            {
                var result = taskOrchestrator.EndTask(taskId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
