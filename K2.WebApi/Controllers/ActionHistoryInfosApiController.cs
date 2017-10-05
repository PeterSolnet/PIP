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
    public class ActionHistoryInfosApiController : ApiController
    {
        private K2Context db = new K2Context();
        public IQueryable<ActionHistoryInfo> GetActionHistoryInfos()
        {
            return db.ActionHistoryInfos;
        }
        [ResponseType(typeof(ActionHistoryInfo))]
        public async Task<IHttpActionResult> GetActionHistoryInfo(long id)
        {
            ActionHistoryInfo actionHistoryInfo = await db.ActionHistoryInfos.FindAsync(id);
            if (actionHistoryInfo == null)
            {
                return NotFound();
            }

            return Ok(actionHistoryInfo);
        }
        [System.Web.Http.Route("api/ActionHistoryInfosApi/{requestId}/requestId")]
        public async Task<List<ActionHistoryInfo>> GetActionHistoryInfoListByRequestId(string requestId)
        {
            var actionInfo = await db.ActionHistoryInfos.Where(x => x.RequestId == requestId).ToListAsync();
            return actionInfo;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActionHistoryInfoExists(long id)
        {
            return db.ActionHistoryInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutActionHistoryInfo(long id, ActionHistoryInfo actionHistoryInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actionHistoryInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(actionHistoryInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionHistoryInfoExists(id))
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
        [ResponseType(typeof(ActionHistoryInfo))]
        public async Task<IHttpActionResult> PostActionHistoryInfo(ActionHistoryInfo actionHistoryInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ActionHistoryInfos.Add(actionHistoryInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = actionHistoryInfo.Id }, actionHistoryInfo);
        }
    }
}