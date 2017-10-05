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
    public class BrdInfosApiController : ApiController
    {
        private K2Context db = new K2Context();
        public IQueryable<BrdInfo> GetBrdInfos()
        {
            return db.BrdInfos;
        }
        [Route("api/BrdInfosApi/{requestId}/brdrequest")]
        public async Task<BrdInfo> GetBrdInfoByRequestId(string requestId)
        {
            var brdInfo = await db.BrdInfos.SingleOrDefaultAsync(x => x.RequestId == requestId);
            return brdInfo;
        }
        [ResponseType(typeof(BrdInfo))]
        public async Task<IHttpActionResult> GetBrdInfo(long id)
        {
            BrdInfo brdInfo = await db.BrdInfos.FindAsync(id);
            if (brdInfo == null)
            {
                return NotFound();
            }

            return Ok(brdInfo);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BrdInfoExists(long id)
        {
            return db.BrdInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBrdInfo(long id, BrdInfo brdInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != brdInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(brdInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrdInfoExists(id))
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
        [ResponseType(typeof(BrdInfo))]
        public async Task<IHttpActionResult> PostBrdInfo(BrdInfo brdInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BrdInfos.Add(brdInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = brdInfo.Id }, brdInfo);
        }
    }
}