using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Configuration.Channels
{
    public class ChannelInfo : ConfigurationElement
    {
        public const string DEFAULT_FORMAT = "Ussd";

        [ConfigurationProperty("MatchIP", IsRequired = true, IsKey = true)]
        public string MatchIP
        {
            get
            {
                return (base["MatchIP"] as string);
            }
            set
            {
                base["MatchIP"] = value;
            }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (base["Name"] as string);
            }
            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("Format", IsRequired = false, DefaultValue = ChannelInfo.DEFAULT_FORMAT)]
        public string Format
        {
            get
            {
                return (base["Format"] as string);
            }
            set
            {
                base["Format"] = value;
            }
        }
    }
}
