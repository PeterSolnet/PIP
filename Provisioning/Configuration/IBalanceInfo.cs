using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Expressions;

namespace Provisioning.Configuration
{
    public interface IBalanceInfo
    {
        string Name { get; set; }
        string AccountIds { get; set; }
        string Divisor { get; set; }
        string DateFormatter { get; set; }
    }
}
