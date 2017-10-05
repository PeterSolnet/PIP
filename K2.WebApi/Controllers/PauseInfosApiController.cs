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
    public class PauseInfosApiController:ApiController
    {
        private K2Context db = new K2Context();
        public IQueryable<PauseInfo> GetPauseInfos()
        {
            return db.PauseInfos;
        }
        [ResponseType(typeof(PauseInfo))]
        public async Task<IHttpActionResult> GetPauseInfo(long id)
        {
            PauseInfo pauseInfo = await db.PauseInfos.FindAsync(id);
            if (pauseInfo == null)
            {
                return NotFound();
            }

            return Ok(pauseInfo);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PauseInfoExists(long id)
        {
            return db.PauseInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPauseInfo(long id, PauseInfo pauseInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pauseInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(pauseInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PauseInfoExists(id))
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
        [ResponseType(typeof(PauseInfo))]
        public async Task<IHttpActionResult> PostPauseInfo(PauseInfo pauseInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PauseInfos.Add(pauseInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = pauseInfo.Id }, pauseInfo);
        }
        [Route("api/PauseInfosApi/{requestId}/{status}/pauseRequest")]
        public async Task<PauseInfo> GetPauseInfoByRequestId(string requestId,string status)
        {    
             PauseInfo pauseInfo = await db.PauseInfos.Where(x => x.RequestId == requestId && x.Status == status).SingleOrDefaultAsync();
            return pauseInfo;
        }
    }
}