using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Provisioning.Helpers
{
    public static class WhiteListHelper
    {
        private const int MaxServiceClass = 10000;
        private static readonly Regex ParseRegex = new Regex(@"(?<from>\d+)(\s*([-:]|to)\s*(?<to>\d+))?", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        /// <summary>
        /// Parse a Service Class range into a list of Service Classes.
        /// </summary>
        /// <param name="serviceClassRange"></param>
        /// <returns></returns>
        public static IEnumerable<int> Parse(string serviceClassRange)
        {
            int from, to;

            foreach (Match m in ParseRegex.Matches(serviceClassRange))
            {
                from = int.Parse(m.Groups["from"].Value);
                to = (string.IsNullOrEmpty(m.Groups["to"].Value) ? from : int.Parse(m.Groups["to"].Value));

                foreach (int i in Enumerable.Range(from, (to - from + 1)))
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Returns a new whitelist from the specified Service Classes.
        /// </summary>
        /// <param name="include">A range of Service Classes to be included in the whitelist.</param>
        /// <param name="exclude">An optional list of Service Classes to exclude from the whitelist.</param>
        /// <returns></returns>
        public static BitArray GetWhiteList(string include, string exclude = "")
        {
            var whitelist = new BitArray(MaxServiceClass, false);

            foreach (var i in Parse(include))
            {
                whitelist.Set(i - 1, true);
            }
            foreach (var i in Parse(exclude))
            {
                whitelist.Set(i - 1, false);
            }

            return whitelist;
        }

        /// <summary>
        /// Checks if the specified serviceClass is included in the whitelist.
        /// </summary>
        /// <param name="whitelist"></param>
        /// <param name="serviceClass"></param>
        /// <returns></returns>
        public static bool IsWhiteListed(BitArray whitelist, int serviceClass)
        {
            serviceClass -= 1;
            if (serviceClass < 0 || serviceClass >= MaxServiceClass)
            {
                throw new ArgumentOutOfRangeException("ServiceClass");
            }

            return whitelist[serviceClass];
        }

        /// <summary>
        /// Returns a new whitelist contining only Service Classes common to the supplied whitelists.
        /// </summary>
        /// <param name="whitelists"></param>
        /// <returns></returns>
        public static BitArray Common(params BitArray[] whitelists)
        {
            BitArray mask = new BitArray(MaxServiceClass, true);
            foreach (var list in whitelists)
            {
                mask = mask.And(list);
            }

            return mask;
        }

        /// <summary>
        /// Returns a new whitelist containing all Service Classes common to the supplied whitelists.
        /// </summary>
        /// <param name="whitelists"></param>
        /// <returns></returns>
        public static BitArray All(params BitArray[] whitelists)
        {
            BitArray mask = new BitArray(MaxServiceClass, false);
            foreach (var list in whitelists)
            {
                mask = mask.Or(list);
            }

            return mask;
        }

        /// <summary>
        /// Returns a set of whitelist names from <paramref name="mapping"/>, which contain the specified <paramref name="serviceClass"/>.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="serviceClass"></param>
        /// <returns></returns>
        public static HashSet<string> WhitelistsForServiceClass(IDictionary<string, BitArray> mapping, int serviceClass)
        {
            serviceClass -= 1;
            var serviceClasses = mapping.Where(x => x.Value[serviceClass]).Select(x => x.Key);
            return new HashSet<string>(serviceClasses);
        }
    }
}
