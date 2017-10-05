using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Provisioning.Configuration
{
    public class ProductSection : ConfigurationSection, IProductSection
    {
        public static ProductSection GetConfig()
        {
            return (ConfigurationManager.GetSection("ProductSection") as ProductSection);
        }

        [ConfigurationProperty("Description", IsRequired = false)]
        public string Description
        {
            get
            {
                return (base["Description"] as string);
            }
        }

        [ConfigurationProperty("DefaultCommandNamespace", IsRequired = false)]
        public string DefaultCommandNamespace
        {
            get
            {
                return (base["DefaultCommandNamespace"] as string);
            }
        }

        [ConfigurationProperty("DefaultReceiver", IsRequired = false)]
        public string DefaultReceiver
        {
            get
            {
                return (base["DefaultReceiver"] as string);
            }
        }

        [ConfigurationProperty("RequestInfos", IsRequired = true)]
        [ConfigurationCollection(typeof(GenericElementCollection<RequestInfo>))]
        public GenericElementCollection<RequestInfo> RequestInfoCollection
        {
            get
            {
                return (base["RequestInfos"] as GenericElementCollection<RequestInfo>);
            }
        }

        public IEnumerable<IRequestInfo> RequestInfos
        {
            get
            {
                return RequestInfoCollection;
            }
        }
    }
}
