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
using System.Web.Mvc;
using K2.WebApi.Models;
using K2.WebApi.Models.Context;
using K2.WebApi.RedisRepo;
using Newtonsoft.Json;

namespace K2.WebApi.Controllers
{
    public class DocumentInfosApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();
        public IQueryable<DocumentInfo> GetDocumentInfos()
        {
            var documentInfoList = db.DocumentInfos;
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            cache.StringSet("k2:documentInfoList", JsonConvert.SerializeObject(documentInfoList));
            return documentInfoList;
        }

      

        [ResponseType(typeof(DocumentInfo))]
        public async Task<IHttpActionResult> GetDocumentInfo(long id)
        {
            DocumentInfo documentInfo = await db.DocumentInfos.FindAsync(id);
            if (documentInfo == null)
            {
                return NotFound();
            }

            return Ok(documentInfo);
        }
        [System.Web.Http.Route("api/DocumentInfosApi/{requestId}/request")]
        [ResponseType(typeof(List<DocumentInfo>))]
        public async Task<IHttpActionResult> GetDocumentListInfoByRequestId(string requestId)
        {
            var documentInfoList = await db.DocumentInfos.Where(x => x.RequestId == requestId).ToListAsync();
            if (documentInfoList==null)
            {
                return NotFound();
            }
            return Ok(documentInfoList);


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocumentInfoExists(long id)
        {
            return db.DocumentInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocumentInfo(long id, DocumentInfo documentInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != documentInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(documentInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentInfoExists(id))
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
        [ResponseType(typeof(DocumentInfo))]
        public async Task<IHttpActionResult> PostDocumentInfo(DocumentInfo documentInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DocumentInfos.Add(documentInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = documentInfo.Id }, documentInfo);
        }
       
        public async Task<IHttpActionResult> PostDocumentInfoSync(DocumentInfo documentInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DocumentInfos.Add(documentInfo);
             db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = documentInfo.Id }, documentInfo);
        }
    }
}