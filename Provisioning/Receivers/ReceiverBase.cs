using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Configuration;

namespace Provisioning.Receivers
{
    public class ReceiverBase : IReceiver, IRequest
    {
        #region IRequest Members

        public IRequestInfo RequestInfo { get; set; }
        public string RequestId { get; set; }
        public string Channel { get; set; }

        #endregion
    }
}
