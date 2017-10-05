using System;
using System.Runtime.Serialization;

namespace Provisioning.Exceptions
{
    public class MultipleCaseMatchException : ApplicationException
    {
        public string[] MatchedCases { get; set; }

        public MultipleCaseMatchException()
            : base()
        {
        }

        public MultipleCaseMatchException(string message, string[] cases)
            : base(message)
        {
            MatchedCases = cases;
        }

        public override string Message
        {
            get
            {
                return string.Format(base.Message,
                    string.Join(", ", MatchedCases));
            }
        }
    }
}