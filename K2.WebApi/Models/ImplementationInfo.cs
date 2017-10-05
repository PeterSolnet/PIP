using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class ImplementationInfo
    {
        public long Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public long ConceptId { get; set; }
        public long BrdInfoId { get; set; }
        public string RequestId { get; set; }
        public bool IsProductReadiness { get; set; }
        public bool IsProductReleaseForTesting { get; set; }
        public string SN { get; set; }
        public string Tag { get; set; }


    }
}