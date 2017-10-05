using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    [Table("PIP_Concept")]
    public class ConceptInfo
    {
        public long Id { get; set; }
        public string OriginatorUserName { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorEmail { get; set; }
        public DateTime CreationDate { get; set; }
        public string CurrentActivityState { get; set; }
        public string Status { get; set; }
        public string ConceptName { get; set; }
        public string ConceptOwner { get; set; }
        public string ProductDescription { get; set; }
        public bool IsNewConcept { get; set; }
        public string Tag { get; set; }
        public string RequestId { get; set; }
        public string SN { get; set; }

    }
}