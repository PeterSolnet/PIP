using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Provisioning.Configuration;
using Provisioning.Expressions;
using SmartFormat;

namespace Provisioning.Helpers
{
    public static class TagHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TagHelper));

        /// <summary>
        /// Returns the user-defined tag for the supplied product configuration.
        /// </summary>
        /// <param name="info">
        /// A <see cref="RequestInfo"/> instance, representing the product configuration.
        /// </param>
        /// <param name="context">The current execution context.</param>
        /// <returns>A string, representing the tag to be applied.</returns>
        public static string GetTag(IRequestInfo info, IExpressionContext context)
        {
            var tag = (string.IsNullOrEmpty(info.Tag) ? info.ExternalData1 : info.Tag);
            try
            {
                return Smart.Format(tag, context.AsDictionary());
            }
            catch (SmartFormat.Core.FormatException exc)
            {
                log.Error(String.Format("Error generating tag: {0} for product: {1}", tag, info.Name), exc);
                throw;
            }
        }
    }
}
