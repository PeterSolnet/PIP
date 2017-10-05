using System;

namespace Provisioning.Expressions
{
    public class Expression

    {
        protected string expr;


        public Expression(string expr)

        {
            this.expr = expr.Trim();
        }


        /// <summary>
        /// Evaluates the expression in the supplied <see cref="ExpressionContext"/>.
        /// </summary>
        /// <param name="context">
        /// The context, used to resolve symbols used in the expression.
        /// </param>
        /// <returns>The result of the evaluation, as an <see cref="object"/>.</returns>
        public object Evaluate(IExpressionContext context)

        {
            if (string.IsNullOrEmpty(this.expr)) return null;

            return context.Evaluate(this.expr);
        }


        public override string ToString()

        {
            return String.Format("{{{0}}}", this.expr);
        }


        public override bool Equals(object obj)

        {
            return this.ToString().Equals(obj.ToString(), StringComparison.Ordinal);
        }


        public override int GetHashCode()

        {
            return this.ToString().GetHashCode();
        }
    }


    public class Expression<T> : Expression, IExpression<T>

    {
        public Expression(string expr)
            : base(expr)

        {
            ;
        }


        /// <summary>
        /// Evaluates the expression in the supplied <see cref="ExpressionContext"/>.
        /// </summary>
        /// <param name="context">
        /// The context, used to resolve symbols used in the expression.
        /// </param>
        /// <returns>
        /// The result of this evaluation, cast as an object of the specified type.
        /// </returns>
        public new T Evaluate(IExpressionContext context)

        {
            object value = base.Evaluate(context);

            return (T) Convert.ChangeType(value, typeof (T));
        }
    }
}