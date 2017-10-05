using Provisioning.Configuration;

namespace Provisioning.Receivers
{
    public interface IRequest
    {
        IRequestInfo RequestInfo { get; set; }
        string RequestId { get; set; }
        string Channel { get; set; }
    }
}