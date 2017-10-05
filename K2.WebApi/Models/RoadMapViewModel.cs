using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class RoadMapViewModel
    {
        public long Id { get; set; }
        public long RoadMapMasterId { get; set; }
        public string RoadMapName { get; set; }
        public string ConceptName { get; set; }
        public string ConceptOwner { get; set; }
    }
}