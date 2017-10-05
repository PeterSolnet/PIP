using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Configuration
{
    public interface ICaseInfo
    {
        string Match { get; set; }
        bool IsDefault { get; set; }
        IEnumerable<ICommandInfo> Commands { get; set; }
    }
}
