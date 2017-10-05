using System.ComponentModel;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class BalanceDetailsInfo : ConfigurationElement, IBalanceInfo
    {
        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string Name
        {
            get
            {
                return (string)base["Name"];

            }
            set
            {
                base["Name"] = value;
            }
        }

        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("AccountIds", IsRequired = true, IsKey = false, DefaultValue = "")]
        public string AccountIds
        {
            get
            {
                return (string)base["AccountIds"];

            }
            set
            {
                base["AccountIds"] = value;
            }
        }

        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("Divisor", IsRequired = true, IsKey = false, DefaultValue = "1024")]
        public string Divisor
        {
            get
            {
                return (string)base["Divisor"];

            }
            set
            {
                base["Divisor"] = value;
            }
        }

        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("DateFormatter", IsRequired = true, IsKey = false, DefaultValue = "DD-MMMM")]
        public string DateFormatter
        {
            get
            {
                return (string)base["DateFormatter"];

            }
            set
            {
                base["DateFormatter"] = value;
            }
        }
    }
}