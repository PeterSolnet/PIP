using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class ImplementationTimeline
    {
        public long Id { get; set; }
        public string ActivityName { get; set; }
        public long? ActivityId { get; set; }
        public string Description { get; set; }
        public DateTime TimelineStartDate { get; set; }
        public DateTime TimelineEndDate { get; set; }
        public string RequestId { get; set; }
        public long ImplementationInfoId { get; set; }
        public string SN { get; set; }
    }
}