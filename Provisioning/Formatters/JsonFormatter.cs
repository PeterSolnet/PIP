using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Provisioning.Commands;


namespace Provisioning.Formatters
{
    public class JSON : IResponseFormatter
    {
        public string ContentType
        {
            get { return "application/json"; }
        }

        public string GetResponse(CommandStatus status)
        {
            // We can cheat here, since we already know that the default
            // string representation of CommandStatus objects is JSON:
            return status.ToString();
        }
    }
}
