using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class PauseInfo
    {
        public long Id { get; set; }
        public string  Tag { get; set; }
        public string RequestId { get; set; }
        public string Actor { get; set; }
        public DateTime? DatePaused { get; set; }
        public DateTime? DateResumed { get; set; }
        public string Status { get; set; }
        public string PauseOriginator { get; set; }
    }
}