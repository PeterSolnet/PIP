using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class OptOutInfoCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new OptOutInfo();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OptOutInfo)element).ServiceClassFromTo;
        }

        public new OptOutInfo this[string serviceClass]
        {
            get
            {
                return (base.BaseGet(serviceClass) as OptOutInfo);
            }
        }

        public OptOutInfo this[int index]
        {
            get
            {
                return (base.BaseGet(index) as OptOutInfo);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }
    }
}
