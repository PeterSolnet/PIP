using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using K2.WebApi.Models;
using K2.WebApi.Services;
using log4net;
using Provisioning.Configuration;
using Provisioning.Configuration.WhiteLists;
using Provisioning.Exceptions;
using Provisioning.Helpers;

namespace Provisioning
{
    public class CachePool
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CachePool));
        private static object @lock = new object();

        private static int counter = 0;
        public static string NextRequestId
        {
            get
            {
                
                return String.Format("{0}{1}", DateTime.Now.ToString("yyMMddHHmmss"), Interlocked.Increment(ref counter));
            }
        }
        public static RequestInfo GetRequestInfo(string message)
        {
            lock (@lock)
            {
                var infos = ProductSection.GetConfig().RequestInfos;
                log.Info(String.Format("Found {0} request infos", infos.Count()));
                foreach (var requestInfo in infos)
                {
                    var item = (RequestInfo) requestInfo;
                    
                    if (new Regex(item.MatchString, RegexOptions.IgnoreCase).Match(message).Success)
                    {
                        item.Parent = ProductSection.GetConfig();
                        return item;
                    }
                }

                throw new RouteNotFound("USSD string {0} was not recognized.", message);
            }
        }

        //static ProductApiService service = new ProductApiService();
        //public static readonly IDictionary<object, ADModel> allADUsers = new Dictionary<object, ADModel>();
        //public static IDictionary<object, ADModel> GetAllAdUserEntries
        //{
        //    get
        //    {
        //        if (allADUsers.Count == 0)
        //        {
        //            lock (@lock)
        //            {
        //                if (allADUsers.Count == 0)
        //                {
        //                    foreach (var item in service.GetADUserList())
        //                    {
        //                        allADUsers.Add(item.UserName,item);
        //                    }
        //                }
        //            }
        //        }
        //        return allADUsers;
        //    }
        //}
        private static Dictionary<string, BitArray> whitelists;
        public static Dictionary<string, BitArray> Whitelists
        {
            get
            {
                if (whitelists == null || whitelists.Count == 0)
                {
                    lock (@lock)
                    {
                        if (whitelists == null || whitelists.Count == 0)
                        {
                            whitelists = new Dictionary<string, BitArray>();
                            foreach (var item in WhiteListSection.GetConfig().WhiteLists)
                            {
                                var list = WhiteListHelper.GetWhiteList(item.Include, item.Exclude);
                                whitelists.Add(item.Name, list);
                            }
                        }
                    }
                }

                return whitelists;
            }
        }
        

        /// <summary>
        /// Checks if a service class is allowed to request a plan.
        /// </summary>
        /// <param name="whitelistKey">
        /// The whitelist specified for the plan (may be null or empty).
        /// </param>
        /// <param name="serviceClass">
        /// The current Service Class of the requesting subscriber.
        /// </param>
        /// //Removed serviceClass
        public static bool IsAllowed(string whitelistKey, int serviceClass)
        {
            if (string.IsNullOrEmpty(whitelistKey))
            {
                // Always return True for non-whitelisted plans:
                return true;
            }

            BitArray whitelist;
            if (Whitelists.TryGetValue(whitelistKey, out whitelist))
            {
                return WhiteListHelper.IsWhiteListed(whitelist, serviceClass);
            }

            throw new ApplicationException(String.Format(@"Whitelist ""{0}"" does not exist.", whitelistKey));
        }

       
    }
}
