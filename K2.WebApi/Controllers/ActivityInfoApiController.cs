using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using K2.WebApi.Models;
using K2.WebApi.Models.Context;

namespace K2.WebApi.Controllers
{
    public class ActivityInfosApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();
        public IQueryable<ActivityInfo> GetActivityInfos()
        {
            return db.ActivityInfos.OrderBy(x => x.DisplayOrder);
        }

        [ResponseType(typeof(ActivityInfo))]
        public async Task<IHttpActionResult> GetActivityInfos(long id)
        {
            ActivityInfo activityInfos = await db.ActivityInfos.FindAsync(id);
            if (activityInfos == null)
            {
                return NotFound();
            }

            return Ok(activityInfos);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityInfoExists(long id)
        {
            return db.ActivityInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutActivityInfo(long id, ActivityInfo activityInfos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activityInfos.Id)
            {
                return BadRequest();
            }

            db.Entry(activityInfos).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityInfoExists(id))
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

        // POST: api/TeamsApi
        [ResponseType(typeof(ActivityInfo))]
        public async Task<IHttpActionResult> PostActivityInfo(ActivityInfo activityInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ActivityInfos.Add(activityInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = activityInfo.Id }, activityInfo);
        }
    }
}