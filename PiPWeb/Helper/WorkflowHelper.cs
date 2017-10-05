using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using log4net;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Workflow.Management;
using SourceCode.Workflow.Management.Criteria;

namespace PiPWeb.Helper
{
    public class WorkflowHelper
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(WorkflowHelper));
        
        public bool ManageWorklistItems()
        {
            //establish the connection
            WorkflowManagementServer k2MgmtServer = new WorkflowManagementServer();
            try
            {
                log.Info("starting ");
                SCConnectionStringBuilder builder =
                    new SCConnectionStringBuilder
                    {
                        Integrated = true,
                        IsPrimaryLogin = true,
                        Authenticate = true,
                        EncryptedPassword = false,
                        Host = "localhost",
                        Port = 5555,
                        SecurityLabelName = "K2"
                    };
                //use the current user's security credentials
                //you must use port 5555 when connecting with the management API
                //this sample uses the Active Directory security provider

                //open the connection using the constructed connection string

                k2MgmtServer.CreateConnection();
                k2MgmtServer.Open(builder.ToString());
                log.Info("Connection Opened ");
                
                //await service.GetK2IssueNameList();
                string k2Prefix = ConfigurationManager.AppSettings["K2Prefix"];
                string theUser = k2Prefix + @"\";
                //Get each user to redirect 
              
                  
                        WorklistCriteriaFilter wlCritFilter = new WorklistCriteriaFilter();
                        wlCritFilter.AddRegularFilter(WorklistFields.Destination, Comparison.Like,
                            "%" + theUser + "mossadmin" + "%");

                        WorklistItems wlItems = k2MgmtServer.GetWorklistItems(wlCritFilter);
                        log.Info("size of " + wlItems.Count);
                        foreach (WorklistItem wlItem in wlItems)
                        {
                            if (wlItem.Status == WorklistItem.WorklistStatus.Allocated || wlItem.Status == WorklistItem.WorklistStatus.Open||wlItem.Status==WorklistItem.WorklistStatus.Sleep)
                            {
                                //k2MgmtServer.ReleaseWorklistItem(wlItem.ID);
                                log.Info("user is "+ theUser + "mossadmin");
                                k2MgmtServer.RedirectWorklistItem(theUser + "mossadmin", theUser + "lekan",
                                wlItem.ProcInstID,
                                wlItem.ActInstDestID,
                                wlItem.ID);
                                log.Info("Redirected Task " + wlItem.Folio + " from Destination " + wlItem.Destination + " to " + theUser + "lekan" + " at " + DateTime.Now);
                            }
                            // k2MgmtServer.ReleaseWorklistItem()
                            //log.Info("Current Destination is : " + wlItem.Destination);
                            //log.Info("from: "+theUser + issue.OldName+"::to :: "+ theUser + issue.NewName);
                            
                        }
                    
                
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Exception occured : " + ex);
                return false;
            }
            //close the connection
            finally
            {
                k2MgmtServer.Connection.Close();
            }
        }
    }
}