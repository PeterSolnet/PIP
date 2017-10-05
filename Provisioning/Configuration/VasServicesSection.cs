using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class VasServicesSection : ConfigurationSection
    {
        public static VasServicesSection GetConfig()
        {
            return (ConfigurationManager.GetSection("VasServicesSection") as VasServicesSection);
        }

        [ConfigurationProperty("Services", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<VasServiceInfo>))]
        public GenericElementCollection<VasServiceInfo> Services
        {
            get
            {
                return (base["Services"] as GenericElementCollection<VasServiceInfo>);
            }
        }
    }
}
