using System;

namespace Provisioning.Configuration
{
    public interface IAirConfiguration
    {
        string AccountInfo { get; }
        int AddressIndicator { get; }
        string HostName { get; }
        string HostType { get; }
        int Port { get; }
        int TimeoutMs { get; }
        string TransactionCurrency { get; }
        string UrlTemplate { get; }
        string UserAgent { get; }
    }
}
