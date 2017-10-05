using System.Collections.Generic;
using System.Configuration;

namespace Provisioning.Configuration
{
    public class CaseInfo : ConfigurationElement, ICaseInfo
    {
        /// <summary>
        /// The value or condition to be matched, for this case's commands to be executed.
        /// </summary>
        [ConfigurationProperty("Match", IsKey = true, IsRequired = false)]
        public string Match
        {
            get
            {
                return base["Match"] as string;
            }
            set
            {
                base["Match"] = value;
            }
        }

        /// <summary>
        /// Marks the default case, executed only if no other cases have been matched.
        /// There can only be one default case -- we'll throw an exception otherwise.
        /// </summary>
        [ConfigurationProperty("IsDefault", IsRequired = false, DefaultValue = false)]
        public bool IsDefault
        {
            get
            {
                return (bool)base["IsDefault"];
            }
            set
            {
                base["IsDefault"] = value;
            }
        }

        /// <summary>
        /// The commands to be executed if this case is matched.
        /// </summary>
        [ConfigurationProperty("Commands", IsRequired = true)]
        [ConfigurationCollection(typeof(GenericElementCollection<CommandInfo>))]
        public GenericElementCollection<CommandInfo> CommandCollection
        {
            get
            {
                return (base["Commands"] as GenericElementCollection<CommandInfo>);
            }
            set
            {
                base["Commands"] = value;
            }
        }

        public IEnumerable<ICommandInfo> Commands
        {
            get
            {
                return CommandCollection;
            }
            set
            {
                CommandCollection = (GenericElementCollection<CommandInfo>)value;
            }
        }
    }
}