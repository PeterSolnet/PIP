using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Expressions;

namespace Provisioning.Configuration
{
    public interface IResponseInfo
    {
        IExpression<bool> When { get; set; }
        string Message { get; set; }
    }
}
