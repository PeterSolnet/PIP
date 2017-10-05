using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Security.UserRoleManager.Management;

namespace PiPWeb.Helper
{
    public class RoleHelper
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(RoleHelper));
        public string GetUserInRole(string roleName)
        {
            string userInRole = String.Empty;
            SCConnectionStringBuilder builder =
                    new SCConnectionStringBuilder
                    {
                        Integrated = true,
                        IsPrimaryLogin = true,
                        Authenticate = true,
                        EncryptedPassword = false,
                        Host = "127.0.0.1",
                        Port = 5555,
                        SecurityLabelName = "K2"
                    };
            Role role = new Role();
            UserRoleManager userRoleManager = new UserRoleManager();
            try
            {
                userRoleManager.CreateConnection();
                userRoleManager.Connection.Open(builder.ToString());
                userInRole = userRoleManager.GetRole(roleName).Include[0].Name;
                log.Info("User in role is " + userInRole);
                userRoleManager.Connection.Close();
                return userInRole;

            }

            catch (Exception ex)
            {
                log.Error("Error in GetUserRole " + ex);
                return userInRole;
            }

        }
    }
}