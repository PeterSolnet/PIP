using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.Runtime.Serialization;

namespace Provisioning.Configuration
{
    /// <summary>
    /// A product configuration.
    /// </summary>
    public class RequestInfo : ConfigurationElement, IRequestInfo
    {
        public IProductSection Parent { get; set; }

        /// <summary>
        /// A regex pattern that matches the incoming message.
        /// </summary>
        [ConfigurationProperty("matchString", IsRequired = true, IsKey = true)]
        public string MatchString
        {
            get
            {
                return (base["matchString"] as string);
            }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (base["name"] as string);
            }
        }

        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get
            {
                return (base["description"] as string);
            }
        }

        [ConfigurationProperty("commandName", IsRequired = false)]
        public string CommandName
        {
            get
            {
                return (base["commandName"] as string);
            }
        }

        [ConfigurationProperty("extra", IsRequired = false)]
        public string Extra
        {
            get
            {
                return (base["extra"] as string);
            }
        }

        [ConfigurationProperty("externalData1", IsRequired = false)]
        public string ExternalData1
        {
            get
            {
                return (base["externalData1"] as string);
            }
        }

        /// <summary>
        /// If provided, will be used to tag the addon subscription record.
        /// You can use SmartFormat-style format parameters in the tag, so long as they resolve to variables in
        /// the current execution context.
        /// </summary>
        [ConfigurationProperty("tag", IsRequired = false)]
        public string Tag
        {
            get
            {
                return (base["tag"] as string);
            }
        }

        [ConfigurationProperty("whitelistKey", IsRequired = false)]
        public string WhitelistKey
        {
            get
            {
                return (base["whitelistKey"] as string);
            }
        }

        [ConfigurationProperty("welcomeBackMessage", IsRequired = false)]
        public string WelcomeBackMessage
        {
            get
            {
                return (base["welcomeBackMessage"] as string);
            }
        }

        [ConfigurationProperty("blacklistMessage", IsRequired = false)]
        public string BlacklistMessage
        {
            get
            {
                return (base["blacklistMessage"] as string);
            }
        }

        [ConfigurationProperty("successMessage", IsRequired = false)]
        public string SuccessMessage
        {
            get
            {
                return (base["successMessage"] as string);
            }
        }

        [ConfigurationProperty("insufficientFundMessage", IsRequired = false)]
        public string InsufficientFundMessage
        {
            get
            {
                return (base["insufficientFundMessage"] as string);
            }
        }

        [ConfigurationProperty("errorMessage", IsRequired = false)]
        public string ErrorMessage
        {
            get
            {
                return (base["errorMessage"].ToString());
            }
        }

        [ConfigurationProperty("ignoreMultipleMigration", IsRequired = false)]
        public bool IgnoreMultipleMigration
        {
            get
            {
                return bool.Parse(base["ignoreMultipleMigration"].ToString());
            }
        }

        [ConfigurationProperty("validationMessage", IsRequired = false)]
        public string ValidationMessage
        {
            get
            {
                return (base["validationMessage"].ToString());
            }
        }

        [ConfigurationProperty("connectionString", IsRequired = false)]
        public string ConnectionString
        {
            get
            {
                return (base["connectionString"].ToString());
            }
        }

        [ConfigurationProperty("DefaultReceiver", IsRequired = false)]
        public string DefaultReceiver
        {
            get
            {
                return (base["DefaultReceiver"] as string);
            }
        }

        [ConfigurationProperty("UseDefaultNameSpace", IsRequired = false, DefaultValue = "true")]
        public bool UseDefaultNameSpace
        {
            get
            {
                return bool.Parse(base["UseDefaultNameSpace"].ToString());
            }
        }

        [ConfigurationProperty("group", IsRequired = false)]
        public string Group
        {
            get
            {
                return (base["group"] as string);
            }
        }

        [ConfigurationProperty("CommandInfos", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<CommandInfo>))]
        public GenericElementCollection<CommandInfo> CommandInfoCollection
        {
            get
            {
                return (base["CommandInfos"] as GenericElementCollection<CommandInfo>);
            }
        }

        public IEnumerable<ICommandInfo> CommandInfos
        {
            get
            {
                return CommandInfoCollection;
            }
        }

        [ConfigurationProperty("Responses", IsRequired = false)]
        [ConfigurationCollection(typeof(GenericElementCollection<ResponseInfo>))]
        public GenericElementCollection<ResponseInfo> ResponseCollection
        {
            get
            {
                return (base["Responses"] as GenericElementCollection<ResponseInfo>);
            }
        }

        public IEnumerable<IResponseInfo> Responses
        {
            get
            {
                return ResponseCollection;
            }
        }

        #region Obsolete Properties: To Be Deleted.

        [Obsolete]
        [ConfigurationProperty("externalData2", IsRequired = false)]
        public string ExternalData2
        {
            get
            {
                return (base["externalData2"] as string);
            }
        }

        //[Obsolete]
        //[ConfigurationProperty("startDate", IsRequired = false)]
        //public DateTime startDate
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(base["startDate"].ToString()))
        //        {
        //            return DateTime.MinValue;
        //        }
        //        return DateTime.ParseExact(base["startDate"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //    }
        //}

        //[Obsolete]
        //[ConfigurationProperty("expiryDate", IsRequired = false)]
        //public DateTime expiryDate
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(base["expiryDate"].ToString()))
        //        {
        //            return DateTime.MaxValue;
        //        }
        //        return DateTime.ParseExact(base["expiryDate"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //    }
        //}

        /// <summary>
        /// If provided, the cost of the plan.
        /// This is DEPRECATED, Use the pair of ValidateBalance and AdjustBalance Commands instead.
        /// </summary>
        [Obsolete("Cost will be removed from future version of this application. Use the pair of ValidateBalance and AdjustBalance Commands instead", false)]
        [ConfigurationProperty("cost", IsRequired = false)]
        public int cost
        {
            get
            {
                return int.Parse(base["cost"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [Obsolete]
        [ConfigurationProperty("term", IsRequired = false)]
        public string term
        {
            get
            {
                return base["term"].ToString();
            }
        }

        [Obsolete]
        [ConfigurationProperty("AccumulatorEnd", IsRequired = false)]
        public int AccumulatorEnd
        {
            get
            {
                return int.Parse(base["AccumulatorEnd"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [Obsolete]
        [ConfigurationProperty("AccumulatorStart", IsRequired = false)]
        public int AccumulatorStart
        {
            get
            {
                return int.Parse(base["AccumulatorStart"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        //added for Vas Services Fulfillment
        [Obsolete]
        [ConfigurationProperty("addServices", IsRequired = false)]
        public bool addServices
        {
            get
            {
                return bool.Parse(base["addServices"].ToString());
            }
        }

        [Obsolete]
        [ConfigurationProperty("internalMigrationCharge", IsRequired = false)]
        public int internalMigrationCharge
        {
            get
            {
                return int.Parse(base["internalMigrationCharge"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [Obsolete]
        [ConfigurationProperty("externalMigrationCharge", IsRequired = false)]
        public int externalMigrationCharge
        {
            get
            {
                return int.Parse(base["externalMigrationCharge"].ToString(), CultureInfo.InvariantCulture);
            }
        }

        [Obsolete]
        [ConfigurationProperty("offerGroup", IsRequired = false)]
        public string offerGroup
        {
            get
            {
                return (base["offerGroup"] as string);
            }
        }

        [Obsolete]
        [ConfigurationProperty("subscriptionDate", IsRequired = false)]
        public string subscriptionDate
        {
            get
            {
                return (base["subscriptionDate"] as string);
            }
        }

        [Obsolete]
        [ConfigurationProperty("trialExpiryDate", IsRequired = false)]
        public string trialExpiryDate
        {
            get
            {
                return (base["trialExpiryDate"] as string);
            }
        }

        [Obsolete]
        [ConfigurationProperty("bonusMessage", IsRequired = false)]
        public string bonusMessage
        {
            get
            {
                return (base["bonusMessage"] as string);
            }
        }

        // added for Offer MigrationCharging Rule
        [Obsolete]
        [ConfigurationProperty("ignoreOfferMigration", IsRequired = false)]
        public bool ignoreOfferMigration
        {
            get
            {
                return bool.Parse(base["ignoreOfferMigration"].ToString());
            }
        }

        #endregion
    }
}
