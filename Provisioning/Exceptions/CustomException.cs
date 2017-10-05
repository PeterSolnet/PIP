using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Exceptions
{
    public class CustomException : ApplicationException
    {
        public CustomException()
            : base()
        { ;}

        public CustomException(string message)
            : base(message)
        { ;}
    }
}
