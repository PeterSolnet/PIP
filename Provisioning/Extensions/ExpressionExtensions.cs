using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Expressions;

namespace Provisioning.Extensions
{
    public static class ExpressionExtensions
    {
        private static string ExprStart = "{";
        private static string ExprEnd = "}";

        /// <summary>
        /// Returns true if the input string looks like an expression (e.g. {name}).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsExpression(this string s)
        {
            return (s.StartsWith(ExprStart) && s.EndsWith(ExprEnd));
        }

        /// <summary>
        /// Converts the input string to a strongly-typed <see cref="Expression&lt;T&gt;"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="checkFirst">
        /// If True, first check if the string looks like an expression (e.g. {name}).
        /// </param>
        /// <returns>A new <see cref="Expression&lt;T&gt;"/>.</returns>
        public static Expression<T> AsExpression<T>(this string s, bool checkFirst = false)
        {
            if (s.IsExpression())
            {
                return new Expression<T>(s.Substring(1, s.Length - 2));
            }
            else
            {
                if (checkFirst) // Shout if this doesn't look like an expression:
                    throw new ArgumentException("String doesn't look like an expression.");
                return new Expression<T>(s);
            }
        }

        /// <summary>
        /// Converts the input string to an <see cref="Expression"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="checkFirst">
        /// If True, first check if the string looks like an expression (e.g. {name}).
        /// </param>
        /// <returns>A new <see cref="Expression"/> instance.</returns>
        public static Expression AsExpression(this string s, bool checkFirst = false)
        {
            return AsExpression<object>(s, checkFirst);
        }
    }
}
