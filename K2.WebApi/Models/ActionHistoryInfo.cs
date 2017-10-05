using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class ActionHistoryInfo
    {
        public long Id { get; set; }
        public string RequestId { get; set; }
        public DateTime ? ActionTimeStamp { get; set; }
        public DateTime ? WorkListArrivalTime { get; set; }
        public string Activity { get; set; } //BRD Line Manager Assign Task
        public string Participant { get; set; } //Action From
        public string DestinationUser { get; set; } //
        public long HeaderId { get; set; }
        public string ProcessName { get; set; } //PIP
        public string SubProcess { get; set; } //BRD
        public string Action { get; set; }//
        public string Comment { get; set; }
        public string RequesterName { get; set; }
    }
}