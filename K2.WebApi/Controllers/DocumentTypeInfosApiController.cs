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
using K2.WebApi.RedisRepo;
using Newtonsoft.Json;

namespace K2.WebApi.Controllers
{
    public class DocumentTypeInfosApiController : ApiController
    {
        // GET api/<controller>
        private K2Context db = new K2Context();
        public IQueryable<DocumentTypeInfo> GetDocumentTypeInfo()
        {
            var documentTypeInfoList = db.DocumentTypeInfos;
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            cache.StringSet("k2:documentTypeInfoList", JsonConvert.SerializeObject(documentTypeInfoList));
            return db.DocumentTypeInfos;
        }
        [Route("api/CachedDocumentTypeInfos")]
        public async Task<List<DocumentTypeInfo>> GetCachedDocumentTypeInfo()
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            string serializedDocumentTypes = cache.StringGet("k2:documentTypeInfoList");
            var documentInfoList = JsonConvert.DeserializeObject<List<DocumentTypeInfo>>(serializedDocumentTypes);
            return documentInfoList;
        }
        [ResponseType(typeof(DocumentTypeInfo))]
        public async Task<IHttpActionResult> GetDocumentTypeInfo(long id)
        {
            DocumentTypeInfo documentTypeInfo = await db.DocumentTypeInfos.FindAsync(id);
            if (documentTypeInfo == null)
            {
                return NotFound();
            }

            return Ok(documentTypeInfo);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocumentTypeInfoExists(long id)
        {
            return db.DocumentTypeInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutK2NameIssue(long id, DocumentTypeInfo documentTypeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != documentTypeInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(documentTypeInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentTypeInfoExists(id))
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
        [ResponseType(typeof(DocumentTypeInfo))]
        public async Task<IHttpActionResult> PostDocumentTypeInfo(DocumentTypeInfo documentTypeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            db.DocumentTypeInfos.Add(documentTypeInfo);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = documentTypeInfo.Id }, documentTypeInfo);
        }
       
    }
}