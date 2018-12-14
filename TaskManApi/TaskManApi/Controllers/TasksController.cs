using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskMan.Business;
using TaskMan.Business.Model;

namespace TaskManApi.Controllers
{
    //[RoutePrefix("api")]
    public class TasksController : ApiController
    {
        ITaskManLogic taskOrchestrator;

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
        public HttpResponseMessage GetAllTasks(int pageIndex, int pageSize)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                var data = taskOrchestrator.GetAllTasks(Page: pageIndex, TotalRecords: pageSize);
                result = Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(ex, true));
            }
            return result;
        }

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
        [HttpPost]
        public IHttpActionResult Post([FromBody]TaskModel newTask)
        {
            if (ModelState.IsValid)
                try
                {
                    taskOrchestrator.AddNewTask(newTask);
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            else
                return BadRequest(ModelState);
        }

        // PUT: Tasks/5
        [HttpPut]
        public IHttpActionResult Put(int taskId, [FromBody]TaskModel value)
        {
            if (ModelState.IsValid)
                try
                {
                    var updatedData = taskOrchestrator.UpdateMyTask(taskId, value);
                    if (updatedData == null)
                        return BadRequest("Update failed - Either the data doesnt exists or invalid request to update");
                    else
                        return Ok(updatedData);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            else
                return BadRequest(ModelState);
        }

        // DELETE: Tasks/5
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
                return InternalServerError(ex);
            }
        }
    }
}
