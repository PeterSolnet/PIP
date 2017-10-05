using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.Runtime.Serialization;

namespace Provisioning.Configuration
{
    public class Override : ConfigurationElement
    {
        [ConfigurationProperty("match", IsRequired = true, IsKey = true)]
        public string Match
        {
            get
            {
                return base["match"] as string;
            }
        }

        [TypeConverter(typeof(BooleanConverter))]
        [ConfigurationProperty("autoRenew", IsRequired = false, DefaultValue = false)]
        public bool AutoRenew
        {
            get
            {
                return (bool)base["autoRenew"];
            }
        }
    }
}
