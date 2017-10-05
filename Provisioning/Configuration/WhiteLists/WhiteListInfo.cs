using System.Configuration;

namespace Provisioning.Configuration.WhiteLists
{
    public class WhiteListInfo : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
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

        [ConfigurationProperty("Include", IsRequired = true)]
        public string Include
        {
            get
            {
                return (base["Include"] as string);
            }
            set
            {
                base["Include"] = value;
            }
        }

        [ConfigurationProperty("Exclude", IsRequired = false, DefaultValue = "")]
        public string Exclude
        {
            get
            {
                return (base["Exclude"] as string);
            }
            set
            {
                base["Exclude"] = value;
            }
        }
    }
}