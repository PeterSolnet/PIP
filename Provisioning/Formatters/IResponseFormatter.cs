using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Commands;


namespace Provisioning.Formatters
{
    public interface IResponseFormatter
    {
        string ContentType { get; }
        string GetResponse(CommandStatus status);
    }
}
