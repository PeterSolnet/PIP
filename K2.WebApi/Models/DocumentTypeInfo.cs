using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class DocumentTypeInfo
    {
        public long Id { get; set; }
        public string DocumentType { get; set; }
        public string Description { get; set; }
        public string DocumentExtension { get; set; }

    }
}