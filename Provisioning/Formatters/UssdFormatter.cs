using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Commands;

namespace Provisioning.Formatters
{
    public class USSD : IResponseFormatter
    {
        public string ContentType
        {
            get { return "text/plain"; }
        }

        public string GetResponse(CommandStatus status)
        {
            return String.Format("<ussd><type>3</type><msg>{0}</msg></ussd>", status.Description);
        }
    }
}
