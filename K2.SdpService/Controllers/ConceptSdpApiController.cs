using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using K2.WebApi.Models;
using log4net;
using Provisioning.Commands;

namespace K2.SdpService.Controllers
{
    public class ConceptSdpApiController : ApiController
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ConceptSdpApiController));

        [Route("api/SdpApi/{userId}/{message}/{conceptName}/{description}/{isNewConcept}/{conceptOwner}/{requestId}/{stakeHolders}")]
        [HttpGet]
        public CommandStatus CreateConcept(string userId, string message, string conceptName, string description, bool isNewConcept, string conceptOwner, string requestId, string stakeHolders)
        {
           
            var args = new NameValueCollection(HttpContext.Current.Request.Headers)
                       {
                           {"userId", userId},
                           {"msg", message},
                           {"conceptName",conceptName},
                            {"description",description},
                            {"isNewConcept",isNewConcept.ToString()},
                            {"conceptOwner",conceptOwner},
                            {"requestId",requestId},
                             {"stakeHolders",stakeHolders}
                            
                       };
            
            return Provisioning.Processor.Instance.Process(args);
        }

        [Route("api/SdpApi/{userId}/{message}/{documentTypeId}/{documentName}/{requestId}")]
        [HttpGet]
        public CommandStatus CreateDocument(string userId, string tag, long documentTypeId, string documentName, string requestId)
        {

            var args = new NameValueCollection(HttpContext.Current.Request.Headers)
                       {
                           {"userId", userId},
                           {"msg", tag},
                           {"documentTypeId",documentTypeId.ToString()},
                            {"documentName",documentName},
                            {"requestId",requestId}
                       };

            return Provisioning.Processor.Instance.Process(args);
        }
        //[Route("api/SdpApi/{documentTypeId}/{documentName}/document")]
       
        [Route("api/SdpApi/{userId}")]
        public string SayHello(string userId)
        {
            return "Hello " + userId;
        }
    }
}