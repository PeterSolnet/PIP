using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class RequestInfoCollection : ConfigurationElementCollection, IEnumerable<RequestInfo>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RequestInfo();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RequestInfo)element).MatchString;
        }

        public new RequestInfo this[string code]
        {
            get
            {
                return (base.BaseGet(code) as RequestInfo);
            }
        }

        public RequestInfo this[int index]
        {
            get
            {
                return (base.BaseGet(index) as RequestInfo);
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

        public new IEnumerator<RequestInfo> GetEnumerator()
        {
            var items = base.GetEnumerator();
            while (items.MoveNext())
            {
                yield return items.Current as RequestInfo;
            }
        }
    }
}
