using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Provisioning.Configuration.Channels;
using Provisioning.Formatters;

namespace Provisioning.Commands.Handlers
{
    public class HandlerFactory : IHttpHandlerFactory
    {
        private static readonly List<IResponseFormatter> formatters;
        private static readonly IResponseFormatter defaultFormatter;

        static HandlerFactory()
        {
            var fmts = new List<IResponseFormatter>();
            IResponseFormatter formatter;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic))
            {
                foreach (var type in assembly.GetExportedTypes()
                    .Where(x => x.GetInterfaces().Any(i => i == typeof(IResponseFormatter))))
                {
                    formatter = (IResponseFormatter)Activator.CreateInstance(type);
                    fmts.Add(formatter);
                    if (type.Name == ChannelInfo.DEFAULT_FORMAT)
                    {
                        defaultFormatter = formatter;
                    }
                }
            }

            formatters = fmts;
        }

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var processorInstance = new Processor();
            //return new ProvisioningHandler(processorInstance, formatters, ChannelInfo.DEFAULT_FORMAT);
            return new ProvisioningHandler(processorInstance, formatters, defaultFormatter);
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
    }
}