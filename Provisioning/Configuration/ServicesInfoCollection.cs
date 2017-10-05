using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class ServicesInfoCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new VasServiceInfo();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((VasServiceInfo)element).Id;
        }

        public new VasServiceInfo this[string matchId]
        {
            get
            {
                return (base.BaseGet(matchId) as VasServiceInfo);
            }
        }

        public VasServiceInfo this[int index]
        {
            get
            {
                return (base.BaseGet(index) as VasServiceInfo);
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
