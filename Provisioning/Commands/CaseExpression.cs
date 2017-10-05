using System;
using System.Text.RegularExpressions;
using Provisioning.Expressions;
using Provisioning.Extensions;

namespace Provisioning.Commands
{
    public class CaseExpression : ICaseExpression
    {
        private static Regex ExpressionRegex = new Regex(@"^(?<var1>\S+)\s+(?<comparison>=|==|!=|<|<=|>|>=)\s+(?<var2>\S+)$", RegexOptions.Compiled);

        private Match matchExpression;
        public string Expression { get; set; }

        /// <summary>
        /// If the supplied parameter is an expression, evaluate it in the supplied context.
        /// Otherwise, return it as-is.
        /// </summary>
        private object GetValue(string var, ExpressionContext context)
        {
            var group = this.matchExpression.Groups[var];
            if (group.Value.IsExpression())
            {
                return group.Value.AsExpression().Evaluate(context);
            }
            else
            {
                return group.Value;
            }
        }

        /// <summary>
        /// Evaluate this expression in the supplied context, returning True or False.
        /// </summary>
        public bool Evaluate(ExpressionContext context)
        {
            this.matchExpression = ExpressionRegex.Match(Expression);
            if (!this.matchExpression.Success)
                throw new ArgumentException("Invalid Case Expression; expected: <var1> <comparison> <var2>.", "expr");

            var groups = this.matchExpression.Groups;
            var var1 = GetValue("var1", context);
            var var2 = GetValue("var2", context);

            var comparison = groups["comparison"].Value;
            switch (comparison)
            {
                case "=":
                case "==":
                    return object.Equals(var1.ToString(), var2.ToString());
                case "!=":
                    return !object.Equals(var1.ToString(), var2.ToString());
                /* The following operators are only supported for numeric operands. */
                case ">":
                    // If there was a `Number` type, we'd use that instead, but there isn't so...
                    return Convert.ToDecimal(var1) > Convert.ToDecimal(var2);
                case ">=":
                    return Convert.ToDecimal(var1) >= Convert.ToDecimal(var2);
                case "<":
                    return Convert.ToDecimal(var1) < Convert.ToDecimal(var2);
                case "<=":
                    return Convert.ToDecimal(var1) <= Convert.ToDecimal(var2);
                default:
                    return false;
            }
        }
    }
}