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
    public class ImplementationTimelinesApiController : ApiController
    {
        private K2Context db = new K2Context();
        public IQueryable<ImplementationTimeline> GetImplementationTimelines()
        {
            return db.ImplementationTimelines;
        }
        [ResponseType(typeof(ImplementationTimeline))]
        public async Task<IHttpActionResult> GetImplementationTimeline(long id)
        {
            ImplementationTimeline implementationTimeline = await db.ImplementationTimelines.FindAsync(id);
            if (implementationTimeline == null)
            {
                return NotFound();
            }

            return Ok(implementationTimeline);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImplementationTimelineExists(long id)
        {
            return db.ImplementationTimelines.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutImplementationTimeline(long id, ImplementationTimeline implementationTimeline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != implementationTimeline.Id)
            {
                return BadRequest();
            }

            db.Entry(implementationTimeline).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImplementationTimelineExists(id))
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
        [ResponseType(typeof(ImplementationTimeline))]
        public async Task<IHttpActionResult> PostImplementationTimeline(ImplementationTimeline implementationTimeline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ImplementationTimelines.Add(implementationTimeline);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = implementationTimeline.Id }, implementationTimeline);
        }

        [ResponseType(typeof(ImplementationTimeline))]
        public async Task<IHttpActionResult> DeleteImplementationTimeline(long id)
        {
            try
            {
                var rowToDelete = await db.ImplementationTimelines.Where(x => x.Id == id).SingleOrDefaultAsync();
                db.ImplementationTimelines.Remove(rowToDelete);
                await db.SaveChangesAsync();
                return Ok(rowToDelete);
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        //Get Implementation Timelines API By Request Id
        [System.Web.Http.Route("api/ImplementationTimelinesApi/{requestId}/requestId")]
        public async Task<List<ImplementationTimeline>> GetImplTimelinesListRequestId(string requestId)
        {
            var implementationTimelineList = await db.ImplementationTimelines.Where(x => x.RequestId == requestId).ToListAsync();
            return implementationTimelineList;
        }

        //Get Implementation Timelines API By Request Id
        [System.Web.Http.Route("api/ImplementationTimelinesApi/{ImplementationInfoId}/ImplementationInfoId")]
        public async Task<List<ImplementationTimeline>> GetImplTimelinesListByImplementationId(long implementationInfoId)
        {
            var implementationTimelineList = await db.ImplementationTimelines.Where(x => x.ImplementationInfoId == implementationInfoId).ToListAsync();
            return implementationTimelineList;
        }
    }
}