using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Expressions
{
    public interface IExpression<T>

    {

        T Evaluate(IExpressionContext context);

    }
}
