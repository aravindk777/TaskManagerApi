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
    [RoutePrefix("tasks")]
    public class TasksController : ApiController
    {
        ITaskManLogic taskOrchestrator;

        public TasksController(ITaskManLogic taskOrchestrator)
        {
            this.taskOrchestrator = taskOrchestrator ?? throw new ArgumentNullException(nameof(taskOrchestrator), "Missing argument while instantiating the controller");
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
        [Route(Name ="GetAll")]
        public IHttpActionResult GetAll(int pageIndex = 0, int pageSize = 0)
        {
            IHttpActionResult result = Ok();
            try
            {
                var data = taskOrchestrator.GetAllTasks(Page: pageIndex, TotalRecords: pageSize);
                result = Ok(data);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // GET: api/Tasks/5
        [HttpGet]
        public IHttpActionResult Get(int taskId)
        {
            IHttpActionResult result = Ok();
            if (ModelState.IsValid)
                try
                {
                    var data = taskOrchestrator.GetTask(taskId);
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
                result = BadRequest();

            return result;
        }

        // POST: api/Tasks
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
                return BadRequest("Invalid Task request");
        }

        // PUT: api/Tasks/5
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
                return BadRequest("Invalid Task Request");
        }

        // DELETE: api/Tasks/5
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
