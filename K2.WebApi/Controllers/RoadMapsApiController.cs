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
    public class RoadMapsApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();

        public IQueryable<RoadMap> GetRoadMaps()
        {
            return db.RoadMaps;
        }

        [ResponseType(typeof(RoadMap))]
        public async Task<IHttpActionResult> GetRoadMap(long id)
        {
            RoadMap roadMap = await db.RoadMaps.FindAsync(id);
            if (roadMap == null)
            {
                return NotFound();
            }

            return Ok(roadMap);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoadMapExists(long id)
        {
            return db.RoadMaps.Count(e => e.Id == id) > 0;
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoadMap(long id, RoadMap roadMap)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roadMap.Id)
            {
                return BadRequest();
            }

            db.Entry(roadMap).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoadMapExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST:
        [ResponseType(typeof(RoadMap))]
        public async Task<IHttpActionResult> PostRoadMap(RoadMap roadMap)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RoadMaps.Add(roadMap);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = roadMap.Id }, roadMap);
        }


        //Get Specific Road Maps
        [System.Web.Http.Route("api/RoadMapsApi/{master}/master")]
        public async Task<List<RoadMap>> GetRoadMapListInfoByMasterId(long master)
        {
            var roadMapList = await db.RoadMaps.Where(x => x.RoadMapMasterId == master).ToListAsync();
            return roadMapList;
        }

       

        //[System.Web.Http.Route("api/GetRoadMapMasterByIdApi/{id}/id")]
        //public async Task<RoadMapMaster> GetRoadMapMasterListInfoByIfd(long id)
        //{
        //    var roadMapList = await db.RoadMapMasters.Include(r => r.Id == id).ToListAsync();
        //    return roadMapList;
        //}
    }
}