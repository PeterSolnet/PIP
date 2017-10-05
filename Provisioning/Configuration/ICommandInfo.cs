using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Configuration
{
    public interface ICommandInfo
    {
        IRequestInfo Parent { get; set; }

        string CommandName { get; }
        string Extra { get; }
        string Name { get; }
        string CheckpointName { get; }
        string Returns { get; }
        string DefaultReceiver { get; }
        bool UseDefaultNameSpace { get; }

        string TestCommand { get; }
        IEnumerable<ICaseInfo> Cases { get; }
        IEnumerable<IAccountInfo> Accounts { get; }

        string Subject { get; }
        string Body { get; }
        string QueryManager { get; }

        bool IsSelectStatement { get; }
    }
}
