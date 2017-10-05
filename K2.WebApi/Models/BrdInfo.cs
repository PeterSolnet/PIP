using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class BrdInfo
    {
        public long Id { get; set; }
        public long ConceptInfoId { get; set; }
        public string ConceptOriginatorLineManager { get; set; }
        public string OriginatingDepartment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReviewMeetingDate { get; set; }
        public long Stakeholder { get; set; }
        public string ExternalReviewer { get; set; }
        public string RequestId { get; set; }
        public long SlaId { get; set; }
        public long? RoadMapId { get; set; }
        public string CurrentActivityState { get; set; }
        public string Status { get; set; }
        public string SN { get; set; }
        public string DestinationUser { get; set; }

    }
}