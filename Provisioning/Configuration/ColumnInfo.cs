using System.ComponentModel;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class ColumnInfo : ConfigurationElement, IColumnInfo
    {
        [TypeConverter(typeof(BooleanConverter))]
        [ConfigurationProperty("IsKey", IsRequired = false, DefaultValue = false)]
        public bool IsKey
        {
            get
            {
                return (bool)base["IsKey"];
            }
            set
            {
                base["IsKey"] = value;
            }
        }

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
        [ConfigurationProperty("Value", IsRequired = false)]
        public string Value
        {
            get
            {
                return (string)base["Value"];
            }
            set
            {
                base["Value"] = value;
            }
        }

        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("ParameterName", IsRequired = false)]
        public string ParameterName
        {
            get
            {
                return (string)base["ParameterName"];
            }
            set
            {
                base["ParameterName"] = value;
            }
        }
        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("QueryOperator", IsRequired = false)]
        public string QueryOperator
        {
            get
            {
                return (string)base["QueryOperator"];
            }
            set
            {
                base["QueryOperator"] = value;
            }
        }
        [TypeConverter(typeof(StringConverter))]
        [ConfigurationProperty("DbParameterType", IsRequired = false)]
        public string DbParameterType
        {
            get
            {
                return (string)base["DbParameterType"];
            }
            set
            {
                base["DbParameterType"] = value;
            }
        }
    }
}