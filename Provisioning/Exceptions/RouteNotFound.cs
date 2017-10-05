using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Exceptions
{
    /// <summary>
    /// The exception thrown when a received message does not match any plan.
    /// </summary>
    public class RouteNotFound : CustomException
    {
        public string ShortCode { get; set; }

        public RouteNotFound()
            : base()
        { ;}

        public RouteNotFound(string message, string shortCode)
            : base(message)
        {
            this.ShortCode = shortCode;
        }

        public override string Message
        {
            get
            {
                return string.Format(base.Message, ShortCode);
            }
        }
    }
}
