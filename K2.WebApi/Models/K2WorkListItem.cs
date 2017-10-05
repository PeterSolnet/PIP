using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class K2WorkListItem
    {
        public long Id { get; set; }
        public string SN { get; set; }
        public string ActivityName { get; set; }
        public string ProcesName { get; set; }
        public string EventName { get; set; }
        public string Folio { get; set; }
        public string OriginatorName { get; set; }
        public long ProcInstId { get; set; }
        public string ViewFlow { get; set; }
        public string Status { get; set; }
        public DateTime EventStartDate { get; set; }
    }
}