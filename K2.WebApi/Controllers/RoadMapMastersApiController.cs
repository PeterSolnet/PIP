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
    public class RoadMapMastersApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();

        public IQueryable<RoadMapMaster> GetRoadMapMaster()
        {
            return db.RoadMapMasters;
        }
        [ResponseType(typeof(RoadMapMaster))]
        public async Task<IHttpActionResult> GetRoadMapMaster(long id)
        {
            RoadMapMaster roadMapMaster = await db.RoadMapMasters.FindAsync(id);
            if (roadMapMaster == null)
            {
                return NotFound();
            }

            return Ok(roadMapMaster);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoadMapMasterExists(long id)
        {
            return db.RoadMapMasters.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoadMapMaster(long id, RoadMapMaster roadMapMaster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roadMapMaster.Id)
            {
                return BadRequest();
            }

            db.Entry(roadMapMaster).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoadMapMasterExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST:
        [ResponseType(typeof(RoadMapMaster))]
        public async Task<IHttpActionResult> PostProdImplTimeline(RoadMapMaster roadMapMaster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RoadMapMasters.Add(roadMapMaster);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = roadMapMaster.Id }, roadMapMaster);
        }
        [System.Web.Http.Route("api/RoadMapMastersApi/{id}/RoadMapMasterInfoByRoadMapMasterId")]
        public async Task<RoadMapMaster> GetRoadMapMasterInfoById(long id)
        {
            var roadMapMasterInfo = await db.RoadMapMasters.Where(x => x.Id == id).SingleOrDefaultAsync();
            return roadMapMasterInfo;
        }
    }
}