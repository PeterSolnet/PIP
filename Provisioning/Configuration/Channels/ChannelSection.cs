using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Configuration.Channels
{
    public class ChannelSection : ConfigurationSection
    {
        public static ChannelSection GetConfig()
        {
            return (ConfigurationManager.GetSection("ChannelSection") as ChannelSection);
        }

        [ConfigurationProperty("DenyUnknownChannel", IsRequired = false, DefaultValue = false)]
        public bool DenyUnknownChannel
        {
            get
            {
                return ((bool)base["DenyUnknownChannel"]);
            }
            set
            {
                base["DenyUnknownChannel"] = value;
            }
        }

        [ConfigurationProperty("Channels", IsRequired = true)]
        public GenericElementCollection<ChannelInfo> Channels
        {
            get
            {
                return (base["Channels"] as GenericElementCollection<ChannelInfo>);
            }
            set
            {
                base["Channels"] = value;
            }
        }
    }
}
