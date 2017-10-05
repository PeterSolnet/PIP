using Provisioning.Expressions;

namespace Provisioning.Commands
{
    public interface ICaseExpression
    {
        string Expression { get; set; }
        bool Evaluate(ExpressionContext context);
    }
}