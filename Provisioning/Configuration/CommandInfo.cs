using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Configuration
{
    public class CommandInfo : ConfigurationElement, ICommandInfo
    {
        public IRequestInfo Parent { get; set; }

        [ConfigurationProperty("CommandName", IsRequired = true)]
        public string CommandName
        {
            get
            {
                return (base["CommandName"] as string);
            }
        }

        [ConfigurationProperty("Extra", IsRequired = false)]
        public string Extra
        {
            get
            {
                return (base["Extra"] as string);
            }
        }

        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (base["Name"] as string);
            }
        }

        [ConfigurationProperty("Checkpoint", IsRequired = false, DefaultValue = null)]
        public string CheckpointName
        {
            get
            {
                return (base["Checkpoint"] as string);
            }
        }

        [ConfigurationProperty("Returns", IsRequired = false, DefaultValue = null)]
        public string Returns
        {
            get
            {
                return (base["Returns"] as string);
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

        // Only used by the SwitchCommand:

        [ConfigurationProperty("TestCommand", IsRequired = false)]
        public string TestCommand
        {
            get
            {
                return (string)base["TestCommand"];
            }
            set
            {
                base["TestCommand"] = value;
            }
        }

        [ConfigurationCollection(typeof(GenericElementCollection<CaseInfo>), AddItemName = "Case")]
        [ConfigurationProperty("Cases", IsRequired = false)]
        public GenericElementCollection<CaseInfo> CaseCollection
        {
            get
            {
                return base["Cases"] as GenericElementCollection<CaseInfo>;
            }
            set
            {
                base["Cases"] = value;
            }
        }

        public IEnumerable<ICaseInfo> Cases
        {
            get
            {
                return CaseCollection;
            }
        }

        // Only used by the UpdateAccountCommand:

        [ConfigurationCollection(typeof(GenericElementCollection<AccountInfo>))]
        [ConfigurationProperty("Accounts", IsRequired = false)]
        public GenericElementCollection<AccountInfo> AccountCollection
        {
            get
            {
                return base["Accounts"] as GenericElementCollection<AccountInfo>;
            }
            set
            {
                base["Accounts"] = value;
            }
        }

        public IEnumerable<IAccountInfo> Accounts
        {
            get
            {
                return AccountCollection;
            }
        }

        // Only used by the SendMailCommand:

        [ConfigurationProperty("Subject", IsRequired = false)]
        public ConfigurationTextElement<string> EmailSubject
        {
            get
            {
                return base["Subject"] as ConfigurationTextElement<string>;
            }
            set
            {
                base["Subject"] = value;
            }
        }

        public string Subject
        {
            get
            {
                return EmailSubject.Value;
            }
        }

        [ConfigurationProperty("Body", IsRequired = false)]
        public ConfigurationTextElement<string> EmailBody
        {
            get
            {
                return base["Body"] as ConfigurationTextElement<string>;
            }
            set
            {
                base["Body"] = value;
            }
        }

        public string Body
        {
            get
            {
                return EmailBody.Value;
            }
        }
        #region Used by the totally generic SqlQueryCommand ~ Ayotunde 
        [ConfigurationProperty("IsSelectStatement", IsRequired = false)]
        public ConfigurationTextElement<Boolean> _IsSelect
        {
            get
            {
                return base["IsSelectStatement"] as ConfigurationTextElement<Boolean>;
            }
            set
            {
                base["IsSelectStatement"] = value;
            }
        }

        public bool IsSelectStatement
        {
            get
            {
                return _IsSelect.Value;
            }
        }

        [ConfigurationProperty("QueryManager", IsRequired = false)]
        public ConfigurationTextElement<string> _QueryManager
        {
            get
            {
                return base["QueryManager"] as ConfigurationTextElement<string>;
            }
            set
            {
                base["QueryManager"] = value;
            }
        }

        public string QueryManager
        {
            get
            {
                return _QueryManager.Value;
            }
        }

        #endregion

        #region for the generic table commands ~ Ayotunde

        [ConfigurationCollection(typeof(GenericElementCollection<ColumnInfo>))]
        [ConfigurationProperty("Columns", IsRequired = false)]
        public GenericElementCollection<ColumnInfo> ColumnInfoCollection
        {
            get
            {
                return base["Columns"] as GenericElementCollection<ColumnInfo>;
            }
            set
            {
                base["Columns"] = value;
            }
        }
        public IEnumerable<IColumnInfo> Columns
        {
            get
            {
                return ColumnInfoCollection;
            }
        }
        #endregion

        #region for the databalance commands ~ Ayotunde

        [ConfigurationCollection(typeof(GenericElementCollection<BalanceDetailsInfo>))]
        [ConfigurationProperty("Balance", IsRequired = false)]
        public GenericElementCollection<BalanceDetailsInfo> BalanceInfoCollection
        {
            get
            {
                return base["Balance"] as GenericElementCollection<BalanceDetailsInfo>;
            }
            set
            {
                base["Balance"] = value;
            }
        }
        public IEnumerable<IBalanceInfo> Balances
        {
            get
            {
                return BalanceInfoCollection;
            }
        }
        #endregion
    }
}
