using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Provisioning.Configuration
{
    /// <summary>
    /// A generic configuration element collection, so we don't need to write boilerplate for every ConfigurationCollection.
    /// </summary>
    /// <typeparam name="T">The <see cref="ConfigurationElement"/> type contained in this collection.</typeparam>
    public class GenericElementCollection<T> : ConfigurationElementCollection, ICollection<T>
        where T:ConfigurationElement, new()
    {
        private PropertyInfo key;

        public GenericElementCollection()
        {
            this.key = typeof(T).GetProperties()
                .Where(p => p.IsDefined(typeof(ConfigurationPropertyAttribute), true))
                .Where(p => ((ConfigurationPropertyAttribute[])p.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), true))[0].IsKey)
                .SingleOrDefault();
            if (this.key == null)
                throw new ConfigurationErrorsException("Element '" + typeof(T).Name + "' does not define a key.");
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return key.GetValue(element, null);
        }

        public T this[object key]
        {
            get
            {
                return (base.BaseGet(key) as T);
            }
            set
            {
                if (base.BaseGet(key) != null)
                {
                    base.BaseRemove(key);
                }
                base.BaseAdd(value);
            }
        }

        public T this[int index]
        {
            get
            {
                return (base.BaseGet(index) as T);
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

        public new IEnumerator<T> GetEnumerator()
        {
            var iter = base.GetEnumerator();
            while (iter.MoveNext())
            {
                yield return iter.Current as T;
            }
        }

        public void Add(T item)
        {
            this[GetElementKey(item)] = item;
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public bool Contains(T item)
        {
            return (base.BaseGet(GetElementKey(item)) != null);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public new bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (base.BaseGet(GetElementKey(item)) == null)
            {
                return false;
            }
            else
            {
                base.BaseRemove(item);
                return true;
            }
        }
    }
}
