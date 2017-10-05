using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using K2.WebApi.Models;
using K2.WebApi.RedisRepo;
using log4net;

namespace K2.WebApi.Controllers
{
    public class AdUserApiController : ApiController
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof (AdUserApiController));
        // GET api/<controller>
        const string KEY_SUBSCRIPTIONS = "k2:subscriptions:{0}";
        public class UserObject
        {
            public string UserInfo { get; set; }
        }

        [System.Web.Http.Route("api/getadusers")]
        public List<string> GetAllAdUsers()
        {
            var userObjectList = new List<UserObject>();
            var userObject = new UserObject();
            var userList = new[] {"Chima", "Okwukwe", "Nkechi", "Bunmi", "Nneka", "Okwechime"};

            // userObjectList.Add();
            return new List<string>(userList);
        }
        [System.Web.Http.Route("api/getcachedadusers")]
        public List<ADModel> GetAdUserCacheList()
        {
           var adUserList=new List<ADModel>();
            try
            {
                var cache = RedisConnectorHelper.Connection.GetDatabase();
                string serializedUsers = cache.StringGet("adUserList");
                adUserList =  JsonConvert.DeserializeObject<List<ADModel>>(serializedUsers);
              
            }
            catch (Exception ex)
            {
                log.Error("Error in GetAdUserCacheList : "+ex);
            }
           
            return adUserList;
        }
        [System.Web.Http.Route("api/getlocaladusers")]
        public List<ADModel> GetLocalAdUser()
        {
            string domainUser = "mossadmin";
            string domainPassword = "p@ssw0rd";
            string domainContext = "TESTPORTAL.LOCAL";
            var adUserList = new List<ADModel>();
            using (var context = new PrincipalContext(ContextType.Domain, domainContext, domainUser, domainPassword))
            {
                try
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll().Where(x => x.DisplayName != null))
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            if (de != null)
                            {
                                var adUser = new ADModel
                                {
                                    FirstName = de.Properties["givenName"].Value,
                                    LastName = de.Properties["sn"].Value,
                                    JobTitle = de.Properties["title"].Value,
                                    UserName = de.Properties["samAccountName"].Value,
                                    Email = de.Properties["userPrincipalName"].Value,
                                    Manager = de.Properties["manager"].Value
                                };
                                var cache = RedisConnectorHelper.Connection.GetDatabase();

                                log.Info("User: " + adUser.UserName);
                                adUserList.Add(adUser);
                                cache.StringSet("adUserList", JsonConvert.SerializeObject(adUserList));
                            }
                        }
                    }
                    return adUserList;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            return new List<ADModel>();
        }

        [System.Web.Http.Route("api/getalladusers")]
        public List<ADModel> GetAdUser()
        {
            string domainUser = "k2developer";
            string domainPassword = "P@ss123*";
            string domainContext = "ETISALATNG.COM";
            var adUserList = new List<ADModel>();
            using (var context = new PrincipalContext(ContextType.Domain, domainContext, domainUser, domainPassword))
            {
                try
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll().Where(x => x.DisplayName != null))
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            if (de != null)
                            {
                                var adUser = new ADModel
                                {
                                    FirstName = de.Properties["givenName"].Value,
                                    LastName = de.Properties["sn"].Value,
                                    JobTitle = de.Properties["title"].Value,
                                    UserName = de.Properties["samAccountName"].Value,
                                    Email = de.Properties["userPrincipalName"].Value,
                                    Manager = de.Properties["manager"].Value
                                };
                                var cache = RedisConnectorHelper.Connection.GetDatabase();

                                log.Info("User: " + adUser.UserName);
                                adUserList.Add(adUser);
                                cache.StringSet("adUserList", JsonConvert.SerializeObject(adUserList));
                            }
                        }
                    }
                    return adUserList;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            return new List<ADModel>();
        }
    }
}