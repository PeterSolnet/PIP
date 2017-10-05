using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class OptOutSection : ConfigurationSection
    {
        public static OptOutSection GetConfig()
        {
            return (ConfigurationManager.GetSection("OptOutSection") as OptOutSection);
        }

        [ConfigurationProperty("DefaultCommandNamespace", IsRequired = false)]
        public string DefaultCommandNamespace
        {
            get
            {
                return (base["DefaultCommandNamespace"] as string);
            }
        }

        [ConfigurationProperty("Description", IsRequired = false)]
        public string Description
        {
            get
            {
                return (base["Description"] as string);
            }
        }

        [ConfigurationProperty("OptOutInfos", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<OptOutInfo>))]
        public GenericElementCollection<OptOutInfo> OptOutInfos
        {
            get
            {
                return (base["OptOutInfos"] as GenericElementCollection<OptOutInfo>);
            }
        }
    }
}
