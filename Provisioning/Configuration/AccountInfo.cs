using System.ComponentModel;
using System.Configuration;
using Provisioning.Expressions;


namespace Provisioning.Configuration
{
    public class AccountInfo : ConfigurationElement, IAccountInfo
    {
        [TypeConverter(typeof(Int32Converter))]
        [ConfigurationProperty("AccountId", IsKey = true, IsRequired = false, DefaultValue = 0)]
        public int AccountId
        {
            get
            {
                return (int)base["AccountId"];
            }
            set
            {
                base["AccountId"] = value;
            }
        }

        [TypeConverter(typeof(BooleanConverter))]
        [ConfigurationProperty("MainAccount", IsRequired = false, DefaultValue = false)]
        public bool MainAccount
        {
            get
            {
                return (bool)base["MainAccount"];
            }
            set
            {
                base["MainAccount"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<decimal>))]
        [ConfigurationProperty("AddValue", IsRequired = false)]
        public IExpression<decimal> AddValue
        {
            get
            {
                return base["AddValue"] as Expression<decimal>;
            }
            set
            {
                base["AddValue"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<decimal>))]
        [ConfigurationProperty("DeductValue", IsRequired = false)]
        public IExpression<decimal> DeductValue
        {
            get
            {
                return base["DeductValue"] as Expression<decimal>;
            }
            set
            {
                base["DeductValue"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<decimal>))]
        [ConfigurationProperty("NewValue", IsRequired = false)]
        public IExpression<decimal> NewValue
        {
            get
            {
                return base["NewValue"] as Expression<decimal>;
            }
            set
            {
                base["NewValue"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<int>))]
        [ConfigurationProperty("Validity", IsRequired = false)]
        public IExpression<int> Validity
        {
            get
            {
                return base["Validity"] as Expression<int>;
            }
            set
            {
                base["Validity"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<bool>))]
        [ConfigurationProperty("Extend", IsRequired = false)]
        public IExpression<bool> Extend
        {
            get
            {
                return base["Extend"] as Expression<bool>;
            }
            set
            {
                base["Extend"] = value;
            }
        }

        [TypeConverter(typeof(TypeConverters.ExpressionTypeConverter<int>))]
        [ConfigurationProperty("Prorate", IsRequired = false)]
        public IExpression<int> Prorate
        {
            get
            {
                return base["Prorate"] as Expression<int>;
            }
            set
            {
                base["Prorate"] = value;
            }
        }
    }
}