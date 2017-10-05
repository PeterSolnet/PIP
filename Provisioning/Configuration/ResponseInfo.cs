using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Provisioning.Expressions;

namespace Provisioning.Configuration
{
    /// <summary>
    /// Specifies a response to a provisioning request.
    /// </summary>
    public class ResponseInfo : ConfigurationElement, IResponseInfo
    {
        /// <summary>
        /// A condition to be evaluated, in the current context, for this message to be returned.
        /// </summary>
        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<bool>))]
        [ConfigurationProperty("When", IsRequired = true, IsKey = true)]
        public IExpression<bool> When
        {
            get
            {
                return base["When"] as Expression<bool>;
            }
            set
            {
                base["When"] = value;
            }
        }

        /// <summary>
        /// The message that will be returned if the above condition is True.
        /// </summary>
        [ConfigurationProperty("Message", IsRequired = true)]
        public string Message
        {
            get
            {
                return base["Message"] as string;
            }
            set
            {
                base["Message"] = value;
            }
        }
    }
}
