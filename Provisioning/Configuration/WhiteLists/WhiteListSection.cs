using System.Configuration;

namespace Provisioning.Configuration.WhiteLists
{
    public class WhiteListSection : ConfigurationSection
    {
        public static WhiteListSection GetConfig()
        {
            return (ConfigurationManager.GetSection("WhiteListSection") as WhiteListSection);
        }

        [ConfigurationProperty("WhiteLists", IsRequired = true)]
        public GenericElementCollection<WhiteListInfo> WhiteLists
        {
            get
            {
                return base["WhiteLists"] as GenericElementCollection<WhiteListInfo>;
            }
            set
            {
                base["WhiteLists"] = value;
            }
        }
    }
}
