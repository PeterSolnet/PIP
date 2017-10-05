using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Provisioning.Configuration
{
    public interface IRequestInfo
    {
        IProductSection Parent { get; set; }

        string MatchString { get; }
        string Name { get; }
        string Description { get; }
        string CommandName { get; }
        string Extra { get; }
        string ExternalData1 { get; }
        string Tag { get; }
        string WhitelistKey { get; }
        string WelcomeBackMessage { get; }
        string BlacklistMessage { get; }
        string SuccessMessage { get; }
        string InsufficientFundMessage { get; }
        string ErrorMessage { get; }
        bool IgnoreMultipleMigration { get; }
        string ConnectionString { get; }
        string DefaultReceiver { get; }
        bool UseDefaultNameSpace { get; }
        string Group { get; }
        IEnumerable<ICommandInfo> CommandInfos { get; }
        IEnumerable<IResponseInfo> Responses { get; }

        // TODO: Remove these properties.
        string ValidationMessage { get; }
        int cost { get; }
    }
}
