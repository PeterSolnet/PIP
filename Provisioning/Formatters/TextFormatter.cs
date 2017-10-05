using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Commands;


namespace Provisioning.Formatters
{
    public class Text : IResponseFormatter
    {
        public string ContentType
        {
            get { return "text/plain"; }
        }

        public string GetResponse(CommandStatus status)
        {
            return status.Description;
        }
    }
}
