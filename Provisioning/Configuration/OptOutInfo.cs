using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class OptOutInfo : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (base["Name"] as string);
            }
        }

        [ConfigurationProperty("ServiceClassFromTo", IsRequired = true, IsKey = true)]
        public string ServiceClassFromTo
        {
            get
            {
                return (base["ServiceClassFromTo"] as string);
            }
        }

        [ConfigurationProperty("CommandInfos", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<CommandInfo>))]
        public GenericElementCollection<CommandInfo> CommandInfos
        {
            get
            {
                return (base["CommandInfos"] as GenericElementCollection<CommandInfo>);
            }
        }
    }
}
