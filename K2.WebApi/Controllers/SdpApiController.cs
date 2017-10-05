using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Provisioning.Commands;

namespace K2.WebApi.Controllers
{
    public class SdpApiController : ApiController
    {
        [Route("api/SdpApi{userId}/{message}")]

        [System.Web.Http.HttpGet]
        public CommandStatus Provision(string userId, string message)
        {
            var args = new NameValueCollection(HttpContext.Current.Request.Headers)
                       {
                           {"userId", userId},
                           {"msg", message}
                       };
            return Provisioning.Processor.Instance.Process(args);
        }
    }
}