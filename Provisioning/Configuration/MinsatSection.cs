using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;

namespace Provisioning.Configuration
{
    [Obsolete("This section will be removed in a future release.")]
    public class MinsatSection : ConfigurationSection
    {
        // Methods
        public static MinsatSection GetConfig()
        {
            return (ConfigurationManager.GetSection("MinsatSection") as MinsatSection);
        }

        // Properties
        [ConfigurationProperty("MinsatCredential", IsRequired = true)]
        public string MinsatCredential
        {
            get
            {
                return base["MinsatCredential"].ToString();
            }
        }

        [ConfigurationProperty("MinsatServer", IsRequired = true)]
        public string MinsatServer
        {
            get
            {
                return base["MinsatServer"].ToString();
            }
        }

        [ConfigurationProperty("MinsatTimeOutMs", IsRequired = true)]
        public int MinsatTimeOutMs
        {
            get
            {
                return int.Parse(base["MinsatTimeOutMs"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [ConfigurationProperty("PortEnd", IsRequired = true)]
        public int PortEnd
        {
            get
            {
                return int.Parse(base["PortEnd"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [ConfigurationProperty("PortStart", IsRequired = true)]
        public int PortStart
        {
            get
            {
                return int.Parse(base["PortStart"].ToString(), CultureInfo.InvariantCulture);
            }
        }
    }
}
