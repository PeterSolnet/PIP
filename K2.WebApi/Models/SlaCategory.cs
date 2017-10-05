using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace K2.WebApi.Models
{
    
    public class SlaCategory
    {
        public long Id { get; set; }
        public string Category { get; set; } //Simple, Medium and Complex
        public string SlaLevel { get; set; } //Product Analyst
        public long MinTimeLimit { get; set; } //For SLA
        public long MaxTimeLimit { get; set; } //For Performance Check
    }
}