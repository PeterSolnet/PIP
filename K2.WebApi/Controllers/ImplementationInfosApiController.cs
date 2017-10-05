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
    public class ImplementationInfosApiController : ApiController
    {
        private K2Context db = new K2Context();
        
        public IQueryable<ImplementationInfo> GetImplementationInfos()
        {
            return db.ImplementationInfos;
        }

        [ResponseType(typeof(ImplementationInfo))]
        public async Task<IHttpActionResult> GetImplementationInfo(long id)
        {
            ImplementationInfo implementationInfo = await db.ImplementationInfos.FindAsync(id);
            if (implementationInfo == null)
            {
                return NotFound();
            }

            return Ok(implementationInfo);
        }

        [Route("api/ImplementationInfosApi/{requestId}/{tag}/GetImplementationByRequestIDandTag")]
        public async Task<ImplementationInfo> GetImplementationInfoByRequestIdandTag(string requestId, string tag)
        {
            var implementationInfo = await db.ImplementationInfos.Where(x => x.RequestId == requestId && x.Tag == tag).SingleOrDefaultAsync();
            return implementationInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImplementationInfoExists(long id)
        {
            return db.ImplementationInfos.Count(e => e.Id == id) > 0;
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutImplementationInfo(long id, ImplementationInfo implementationInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != implementationInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(implementationInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImplementationInfoExists(id))
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
        [ResponseType(typeof(ImplementationInfo))]
        public async Task<IHttpActionResult> PostImplementationInfo(ImplementationInfo implementationInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ImplementationInfos.Add(implementationInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = implementationInfo.Id }, implementationInfo);
        }

    }
}