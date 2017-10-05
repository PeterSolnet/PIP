using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class DocumentInfo
    {
        public long Id { get; set; }
        public long DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string RequestId { get; set; }
        public string DocumentDescription { get; set; }
        public string Tag { get; set; }

    }
}