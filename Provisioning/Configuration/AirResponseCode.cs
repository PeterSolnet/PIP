using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Provisioning.Configuration
{
    public class AirResponseCode : ConfigurationElement
    {
        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("Code", IsRequired = true, IsKey = true)]
        public int Code
        {
            get
            {
                return (int)base["Code"];
            }
        }

        [ConfigurationProperty("Description", IsRequired = true)]
        public string Description
        {
            get
            {
                return base["Description"] as string;
            }
        }
    }
}
