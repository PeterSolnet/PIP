using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Expressions;

namespace Provisioning.Configuration
{
    public interface IColumnInfo
    {
        bool IsKey { get; set; }
        string Name { get; set; }
        string Value { get; set; }
        string ParameterName { get; set; }
        string QueryOperator { get; set; }
        string DbParameterType { get; set; }
    }
}
