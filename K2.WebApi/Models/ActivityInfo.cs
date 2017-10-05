using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class ActivityInfo
    {
        public long Id { get; set; }
        public string ActivityName { get; set; }
        public int DisplayOrder { get; set; }
    }
}