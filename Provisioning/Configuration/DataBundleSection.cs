using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.Runtime.Serialization;

namespace Provisioning.Configuration
{
    public class DataBundleSection : ConfigurationSection
    {
        public static DataBundleSection GetSection()
        {
            return (ConfigurationManager.GetSection("DataBundleConfiguration") as DataBundleSection);
        }

        [ConfigurationProperty("autoRenew", IsRequired = true)]
        public bool AutoRenew
        {
            get
            {
                return bool.Parse(base["autoRenew"].ToString());
            }
        }

        [ConfigurationProperty("appendOnSuccess", IsRequired = true)]
        public string AppendOnSuccess
        {
            get
            {
                return base["appendOnSuccess"].ToString();
            }
        }

        [ConfigurationProperty("overrides", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<Override>))]
        public GenericElementCollection<Override> Overrides
        {
            get
            {
                return base["overrides"] as GenericElementCollection<Override>;
            }
            set
            {
                base["overrides"] = value;
            }
        }

        [ConfigurationProperty("notifications", IsRequired = true)]
        [ConfigurationCollection(typeof(GenericElementCollection<Notification>))]
        public GenericElementCollection<Notification> Notifications
        {
            get
            {
                return base["notifications"] as GenericElementCollection<Notification>;
            }
            set
            {
                base["notifications"] = value;
            }
        }
    }
}
