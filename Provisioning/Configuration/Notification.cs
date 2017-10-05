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
    public class Notification : ConfigurationElement
    {
        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("days", IsRequired = true, IsKey = true)]
        public int Days
        {
            get
            {
                return (int)base["days"];
            }
        }

        [ConfigurationProperty("message", IsRequired = true)]
        public string Message
        {
            get
            {
                return base["message"] as string;
            }
        }
    }
}
