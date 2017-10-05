using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using K2.WebApi.Models;
using K2.WebApi.Models.Context;
using log4net;

namespace K2.WebApi.Controllers
{
    public class ConceptInfosApiController : ApiController
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ConceptInfosApiController));

        public ConceptInfosApiController()
        {
        }

        // GET api/<controller>
        public K2Context db = new K2Context();
        public IQueryable<ConceptInfo> GetConceptInfos()
        {
            return db.ConceptInfos;
        }



        [ResponseType(typeof(ConceptInfo))]
        public async Task<IHttpActionResult> GetConceptInfo(long id)
        {
            ConceptInfo conceptInfo = await db.ConceptInfos.FindAsync(id);
            if (conceptInfo == null)
            {
                return NotFound();
            }

            return Ok(conceptInfo);
        }
        [Route("api/ConceptInfosApi/{requestId}/conceptrequest")]
        public async Task<ConceptInfo> GetConceptInfoByRequestId(string requestId)
        {
            var conceptInfo = await db.ConceptInfos.SingleOrDefaultAsync(x => x.RequestId == requestId);
            return conceptInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ConceptInfoExists(long id)
        {
            return db.ConceptInfos.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutConceptInfo(long id, ConceptInfo conceptInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != conceptInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(conceptInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConceptInfoExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TeamsApi
        [ResponseType(typeof(ConceptInfo))]
        public async Task<IHttpActionResult> PostConceptInfo(ConceptInfo conceptInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ConceptInfos.Add(conceptInfo);
            await db.SaveChangesAsync();
            //db.SaveChanges();
             
            return CreatedAtRoute("DefaultApi", new { id = conceptInfo.Id }, conceptInfo);
        }
        [Route("api/ConceptInfosApi/{conceptInfo}/newconceptInfo")]
        [ResponseType(typeof(ConceptInfo))]
        public async Task<IHttpActionResult> PostNewConceptInfo(string userId, string conceptName, string description, bool isNewConcept, string conceptOwner, string requestId)
        {
            var conceptInfo = new ConceptInfo
            {
                OriginatorUserName = userId,
                ConceptName = conceptName,
                ConceptOwner = conceptOwner,
                IsNewConcept =isNewConcept,
                ProductDescription = description,
                RequestId = requestId
                
            };
            //string userId, string message, string conceptName, string description, bool isNewConcept, string conceptOwner, string requestId
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ConceptInfos.Add(conceptInfo);
             await db.SaveChangesAsync();
            log.Info("Added to database");
            return CreatedAtRoute("DefaultApi", new { id = conceptInfo.Id }, conceptInfo);
        }
    }
}