using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class VasServiceInfo : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get
            {
                return (base["Id"] as string);
            }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (base["Name"] as string);
            }
        }

        //[ConfigurationProperty("Description", IsRequired = true)]
        //public string Description
        //{
        //    get
        //    {
        //        return (base["Description"] as string);
        //    }
        //}

        [ConfigurationProperty("TrialDate", IsRequired = true)]
        public string TrialDate
        {
            get
            {
                return (base["TrialDate"] as string);
            }
        }

        [ConfigurationProperty("Url", IsRequired = true)]
        public string Url
        {
            get
            {
                return (base["Url"] as string);
            }
        }

        //[ConfigurationProperty("Extra", IsRequired = true)]
        //public string Extra
        //{
        //    get
        //    {
        //        return (base["Extra"] as string);
        //    }
        //}

        [ConfigurationProperty("Response", IsRequired = true)]
        public string Response
        {
            get
            {
                return (base["Response"] as string);
            }
        }

        [ConfigurationProperty("SuccessMessage", IsRequired = false)]
        public string SuccessMessage
        {
            get
            {
                return (base["SuccessMessage"] as string);
            }
        }
    }
}
