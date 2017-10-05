using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class RoadMap
    {
        public long Id { get; set; }
        public long RoadMapMasterId { get; set; }
        public string ConceptName { get; set; } //Concept Name from Concept Stage
        public string ConceptOwner { get; set; } 

    }
}