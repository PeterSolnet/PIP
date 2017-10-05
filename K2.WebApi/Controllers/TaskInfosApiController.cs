using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using K2.WebApi.Models;
using K2.WebApi.Models.Context;

namespace K2.WebApi.Controllers
{
    public class TaskInfosApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();
        public IQueryable<TaskInfo> GetTaskInfos()
        {
            return db.TaskInfos;
        }
        [ResponseType(typeof(TaskInfo))]
        public async Task<IHttpActionResult> GetTaskInfo(long id)
        {
            TaskInfo taskInfo = await db.TaskInfos.FindAsync(id);
            if (taskInfo == null)
            {
                return NotFound();
            }

            return Ok(taskInfo);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskInfoExists(long id)
        {
            return db.TaskInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTaskInfo(long id, TaskInfo taskInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(taskInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskInfoExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TeamsApi
        [ResponseType(typeof(TaskInfo))]
        public async Task<IHttpActionResult> PostTaskInfo(TaskInfo taskInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskInfos.Add(taskInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taskInfo.Id }, taskInfo);
        }

        [System.Web.Http.Route("api/TaskInfosApi/{requestId}/request")]
        public async Task<List<TaskInfo>> GetTaskInfoListByRequestId(string requestId)
        {
            var taskInfoList = await db.TaskInfos.Where(x => x.RequestId == requestId).ToListAsync();
            return taskInfoList;
        }
    }
 
}