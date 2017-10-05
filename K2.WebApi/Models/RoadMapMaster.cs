using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class RoadMapMaster
    {
        public long Id { get; set; }
        public string RoadMapName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}