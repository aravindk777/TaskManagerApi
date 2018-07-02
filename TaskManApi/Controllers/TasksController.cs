using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TaskManApi.Data;
using TaskManApi.Models;

namespace TaskManApi.Controllers
{
    public class TasksController : ApiController
    {
        private ITaskManDb db;// = new TaskManDb();

        public TasksController(ITaskManDb _taskManDb)
        {
            db = _taskManDb;
        }

        //public TasksController()
        //{
        //    db = new TaskManDb();
        //}

        // GET: api/Tasks
        /// <summary>
        /// Get all Tasks from the DB
        /// </summary>
        /// <returns>List of tasks - IQuerable</returns>
        public IQueryable<Task> GetTasks()
        {
            return db.Tasks;
        }

        // GET: api/Tasks/5
        [ResponseType(typeof(Task))]
        public IHttpActionResult GetTask(Guid id)
        {
            Task task = db.Tasks.FirstOrDefault(t => t.TaskId.Equals(id));  //db.Tasks.Find(id);
            if (task == null)
            {
                return Ok(task);
                //return Ok(string.Format("No task available for the Id [{0}]", id));
            }

            return Ok(task);
        }

        // PUT: api/Tasks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTask(Task task)
        {
            if (task.TaskId.Equals(Guid.NewGuid()))
                return BadRequest("Id is invalid!");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTask = db.Tasks.Find(task.TaskId);
            if (!task.TaskId.Equals(existingTask.TaskId))
                return NotFound();

            if (task.ParentTaskId.HasValue) existingTask.ParentTaskId = task.ParentTaskId;
            if (!string.IsNullOrEmpty(task.TaskName) && !existingTask.TaskName.Equals(task.TaskName)) existingTask.TaskName = task.TaskName;
            existingTask.Priority = task.Priority;
            if (task.StartDate.HasValue) existingTask.StartDate = task.StartDate;
            if (task.EndDate.HasValue) existingTask.EndDate = task.EndDate;

            ((TaskManDb) db).Entry(existingTask).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.TaskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Tasks
        [ResponseType(typeof(Task))]
        public IHttpActionResult PostTask(Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            task.TaskId = Guid.NewGuid();
            
            if (task.StartDate.HasValue)
            {
                if (task.EndDate.HasValue && !task.EndDate.Value.Equals(DateTime.MinValue) && task.EndDate.Value.CompareTo(task.StartDate.Value) <= 0)
                    return BadRequest("End date cannot be past than the start date");
            }

            db.Tasks.Add(task);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TaskExists(task.TaskId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = task.TaskId }, task);
        }

        // DELETE: api/Tasks/5
        [ResponseType(typeof(Task))]
        public IHttpActionResult DeleteTask(Guid id)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return Ok(task);
            }
            try
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
            catch(DbUpdateException updateEx)
            {
                return Conflict();
            }

            return Ok(task);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskExists(Guid id)
        {
            return db.Tasks.Count(e => e.TaskId == id) > 0;
        }
    }
}