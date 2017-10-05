using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Provisioning.Formatters;

namespace Provisioning.Commands.Handlers
{
    public class ProvisioningHandler : IHttpHandler
    {
        private Processor processorInstance;
        private List<IResponseFormatter> formatters;
        private IResponseFormatter defaultFormatter;

        public ProvisioningHandler(Processor processor, List<IResponseFormatter> formatters, IResponseFormatter defaultFormat)
        {
            processorInstance = processor;
            this.formatters = formatters;
            defaultFormatter = defaultFormat;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextWrapper(context));
        }

        public void ProcessRequest(HttpContextBase context)
        {
            var args = new NameValueCollection
                       {
                           context.Request.Headers,
                           context.Request.QueryString,
                           context.Request.Form
                       };

            var status =  processorInstance.Process(args);
            var format = args["format"] ?? args["X-Channel-Format"];
            var formatter = formatters.SingleOrDefault(x =>
                string.Compare(x.GetType().Name, format, StringComparison.OrdinalIgnoreCase) == 0);

            if (null == formatter)
            {
                //formatter = formatters.Single(x => x.GetType().Name == defaultFormatter);
                formatter = defaultFormatter;
            }

            var response = formatter.GetResponse(status);
            context.Response.ContentType = formatter.ContentType;
            context.Response.AppendHeader("Content-Length", response.Length.ToString());
            context.Response.Write(response);
            context.Response.Flush();
        }
    }
}