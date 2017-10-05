using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class StakeHolder
    {
        public long Id { get; set; }
        public string StakeHolderName { get; set; }
        public string Tag { get; set; }
        public string RequestId { get; set; }
    }
}