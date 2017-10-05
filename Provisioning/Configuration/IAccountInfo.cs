using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Provisioning.Expressions;

namespace Provisioning.Configuration
{
    public interface IAccountInfo
    {
        int AccountId { get; set; }
        bool MainAccount { get; set; }
        IExpression<decimal> AddValue { get; set; }
        IExpression<decimal> DeductValue { get; set; }
        IExpression<decimal> NewValue { get; set; }
        IExpression<int> Validity { get; set; }
        IExpression<bool> Extend { get; set; }
        IExpression<int> Prorate { get; set; }
    }
}
