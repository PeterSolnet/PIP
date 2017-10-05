using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;

namespace Provisioning.Configuration
{
    public class AirSection : ConfigurationSection, IAirConfiguration
    {
        public static AirSection GetConfig()
        {
            return (ConfigurationManager.GetSection("AirSection") as AirSection);
        }

        [ConfigurationProperty("AccountInfo", IsRequired = true)]
        public string AccountInfo
        {
            get
            {
                return base["AccountInfo"].ToString();
            }
        }

        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("AddressIndicator", IsRequired = true)]
        public int AddressIndicator
        {
            get
            {
                return (int)base["AddressIndicator"];
            }
        }

        [ConfigurationProperty("ExternalData1", IsRequired = false)]
        public string ExternalData1
        {
            get
            {
                return base["ExternalData1"].ToString();
            }
        }

        [ConfigurationProperty("HostName", IsRequired = true)]
        public string HostName
        {
            get
            {
                return base["HostName"].ToString();
            }
        }

        [ConfigurationProperty("HostType", IsRequired = true)]
        public string HostType
        {
            get
            {
                return base["HostType"].ToString();
            }
        }

        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("Port", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)base["Port"];
            }
        }

        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("TimeOutMs", IsRequired = true)]
        public int TimeoutMs
        {
            get
            {
                return (int)base["TimeOutMs"];
            }
        }

        [ConfigurationProperty("TransactionCurrency", IsRequired = true)]
        public string TransactionCurrency
        {
            get
            {
                return base["TransactionCurrency"].ToString();
            }
        }

        [ConfigurationProperty("UrlTemplate", IsRequired = true)]
        public string UrlTemplate
        {
            get
            {
                return base["UrlTemplate"].ToString();
            }
        }

        [ConfigurationProperty("UserAgent", IsRequired = true)]
        public string UserAgent
        {
            get
            {
                return base["UserAgent"].ToString();
            }
        }

        

        [ConfigurationProperty("ResponseCodes", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<AirResponseCode>))]
        public GenericElementCollection<AirResponseCode> ResponseCodes
        {
            get
            {
                return base["ResponseCodes"] as GenericElementCollection<AirResponseCode>;
            }
        }
    }
}
