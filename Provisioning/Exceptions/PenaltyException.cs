using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Exceptions
{
    /// <summary>
    /// The exception thrown when a subscriber cannot pay a migration penalty.
    /// </summary>
    public class PenaltyException : CustomException
    {
        public PenaltyException()
            : base()
        { ;}

        public PenaltyException(string message)
            : base(message)
        { ;}
    }
}
