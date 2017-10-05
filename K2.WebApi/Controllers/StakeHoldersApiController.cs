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
    public class StakeHoldersApiController:ApiController
    {
        private K2Context db = new K2Context();
        public IQueryable<StakeHolder> GetStakeHolders()
        {
            return db.StakeHolders;
        }
        [ResponseType(typeof(StakeHolder))]
        public async Task<IHttpActionResult> GetStakeHolder(long id)
        {
            StakeHolder stakeHolder = await db.StakeHolders.FindAsync(id);
            if (stakeHolder == null)
            {
                return NotFound();
            }

            return Ok(stakeHolder);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StakeHolderExists(long id)
        {
            return db.StakeHolders.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStakeHolder(long id, StakeHolder stakeHolder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stakeHolder.Id)
            {
                return BadRequest();
            }

            db.Entry(stakeHolder).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StakeHolderExists(id))
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
        [ResponseType(typeof(StakeHolder))]
        public async Task<IHttpActionResult> PostStakeHolder(StakeHolder stakeHolder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.StakeHolders.Add(stakeHolder);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = stakeHolder.Id }, stakeHolder);
        }
    }
}