using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Provisioning.Configuration
{
    public interface IProductSection
    {
        string DefaultCommandNamespace { get; }
        string DefaultReceiver { get; }
        string Description { get; }
        IEnumerable<IRequestInfo> RequestInfos { get; }
    }
}
