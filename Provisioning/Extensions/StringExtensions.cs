using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string containing the leftmost <paramref name="len"/> characters of this string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len">
        /// The number of characters to return. If greater than the length of the string, the entire string is returned.
        /// </param>
        /// <returns>A new string</returns>
        public static string Left(this string str, int len)
        {
            return str.Substring(0, Math.Min(len, str.Length));
        }

        /// <summary>
        /// Returns a new string containing the rightmost <paramref name="len"/> characters of this string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len">
        /// The number of characters to return. If greater than the length of the string, the entire string is returned.
        /// </param>
        /// <returns>A new string</returns>
        public static string Right(this string str, int len)
        {
            return str.Substring(Math.Max((str.Length - len), 0));
        }

        /// <summary>
        /// Converts a comma separated string of ints e.g. 1,2,3,4,5 to a generic List of ints
        /// </summary>
        /// <param name="str">A comma separated list of ints</param>
        /// <returns>List of ints in the given string</returns>
        public static List<int> ToIntArray(this string str)
        {
            string[] splitted = System.Text.RegularExpressions.Regex.Split(str, ",");
            return Array.ConvertAll<string, int>(splitted,
                int.Parse).ToList();
        }
    }
}
