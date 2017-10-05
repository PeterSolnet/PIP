using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using K2.WebApi.Models;
using K2.WebApi.Models.Context;

namespace K2.WebApi.Controllers
{
    public class SlaCategoriesApiController : ApiController
    {

        // GET api/<controller>
        private K2Context db = new K2Context();
        
        public IQueryable<SlaCategory> GetSlaCategories()
        {
            return db.SlaCategories;
        }
        [ResponseType(typeof(SlaCategory))]
        public async Task<IHttpActionResult> GetSlaCategory(long id)
        {
            SlaCategory slacategory = await db.SlaCategories.FindAsync(id);
            if (slacategory == null)
            {
                return NotFound();
            }

            return Ok(slacategory);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SlaCategoryExists(long id)
        {
            return db.SlaCategories.Count(e => e.Id == id) > 0;
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSlaCategory(long id, SlaCategory slacategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != slacategory.Id)
            {
                return BadRequest();
            }

            db.Entry(slacategory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlaCategoryExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST:
        [ResponseType(typeof(SlaCategory))]
        public async Task<IHttpActionResult> PostSlaCategory(SlaCategory slacategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SlaCategories.Add(slacategory);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = slacategory.Id }, slacategory);
        }
    }
}
