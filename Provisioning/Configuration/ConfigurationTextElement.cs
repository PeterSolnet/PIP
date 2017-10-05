using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace Provisioning.Configuration
{
    /// <summary>
    /// A configuration element that stores its data as element text, rather than in attributes.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> type returned by this element.</typeparam>
    public class ConfigurationTextElement<T> : ConfigurationElement
    {
        private T value;

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            value = (T)reader.ReadElementContentAs(typeof(T), null);
        }

        public T Value
        {
            get { return value; }
        }
    }
}
