using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K2.WebApi.Models
{
    public class TaskInfo
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public string Assignee { get; set; }
        public string AssignedBy { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string AssignmentType { get; set; } //Original or Re-assigned
        public string TaskPriority { get; set; } //High, Medium or Low
        public string RequestId { get; set; }
        public string Tag { get; set; } //Concept, BRD or Implementation
        public string SN { get; set; }
    }
}