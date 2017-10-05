using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Provisioning.Commands;
using System.Web;
using log4net;

namespace Provisioning.Helpers
{
    public class ProvisionHelper
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(ProvisionHelper));
        private string baseUrl;

        public ProvisionHelper(string url)
        {
            this.baseUrl = url;
        }

        public CommandStatus Provision(Dictionary<string, string> args)
        {
            args["format"] = "json"; // Force JSON output.
            var strArgs = string.Join("&", args.Select(x =>
                String.Format("{0}={1}", x.Key, HttpUtility.UrlPathEncode(x.Value))).ToArray());
            var url = String.Format("{0}?{1}", baseUrl, HttpUtility.UrlPathEncode(strArgs));
            log.Info("URL passed is "+url);
            var req = WebRequest.Create(url);

            string response = string.Empty;

            using (var stm = req.GetResponse().GetResponseStream())
            {
                var rdr = new StreamReader(stm);
                response = rdr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<CommandStatus>(response);
        }
        public CommandStatus ProvisionConcept(Dictionary<string, string> args)
        {
            //args["format"] = "json"; // Force JSON output.
            var strArgs = string.Join("/", args.Select(x =>
                String.Format("{1}", x.Key, HttpUtility.UrlPathEncode(x.Value))).ToArray());
            var url = String.Format("{0}/{1}", baseUrl, strArgs);
            //log.Info("URL passed is " + url);
            var req = WebRequest.Create(url);

            string response = string.Empty;
            log.Info("The url passed is "+url);
            using (var stm = req.GetResponse().GetResponseStream())
            {
                var rdr = new StreamReader(stm);
                response = rdr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<CommandStatus>(response);
        }
    }
}
