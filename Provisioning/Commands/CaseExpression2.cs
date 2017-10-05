using Provisioning.Expressions;
using Provisioning.Extensions;

namespace Provisioning.Commands
{
    public class CaseExpression2 : ICaseExpression
    {
        public string Expression { get; set; }

        public bool Evaluate(ExpressionContext context)
        {
            return (bool)Expression.AsExpression().Evaluate(context);
        }
    }
}