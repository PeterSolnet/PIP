using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using K2.WebApi.Controllers;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using PiPWeb.Helper;
using PiPWeb.Services;
using Provisioning.Commands;
using Provisioning.Commands.Model;
using Provisioning.Helpers;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Security.UserRoleManager.Management;
using SourceCode.Workflow.Client;


namespace PiPWeb.Controllers
{
    public class ConceptController : Controller
    {
        ConceptInfosApiController conceptInfosApiController = new ConceptInfosApiController();
        DocumentHelper helper = new DocumentHelper();
        // GET: /Concept/
        protected static readonly ILog log = LogManager.GetLogger(typeof (ConceptController));

        public string GetUserInRole()
        {
            string  userInRole=String.Empty;
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
                 userInRole = userRoleManager.GetRole("Concept Review").Include[0].Name;
                log.Info("User in role is " + userInRole);
                userRoleManager.Connection.Close();
                return userInRole;

            }

            catch (Exception ex)
            {
                log.Error("Error in GetUserRole "+ex);
                return userInRole;
            }
            
        }

        public ActionResult PipWorkList()
        {
            string impersonatedUser = @"K2:"+User.Identity.Name;
            var theList = new List<K2WorkListItem>();
            var k2worklistItem = new K2WorkListItem();
            //Establish a connection
            using (var connection = new Connection())
            {
                connection.Open("localhost");
                connection.ImpersonateUser(impersonatedUser);
                Worklist worklist = connection.OpenWorklist("ASP");


                foreach (WorklistItem worklistItem in worklist)
                {

                    k2worklistItem = new K2WorkListItem
                    {
                        SN = worklistItem.SerialNumber,
                        ProcesName = worklistItem.ProcessInstance.FullName,
                        ActivityName = worklistItem.ActivityInstanceDestination.Name,
                        EventName = worklistItem.EventInstance.Name,
                        EventStartDate = worklistItem.EventInstance.StartDate,
                        Folio = worklistItem.ProcessInstance.Folio,
                        ViewFlow = worklistItem.ProcessInstance.ViewFlow,
                        OriginatorName = worklistItem.ProcessInstance.Originator.Name,
                        ProcInstId = worklistItem.ActivityInstanceDestination.ProcInstID,
                        Status = worklistItem.Status.ToString()

                    };
                    
                    log.Info("Allocated user" + worklistItem.AllocatedUser);
                    
                    theList.Add(k2worklistItem);
                }

                connection.Close();
           }

            return View(theList);
        }
        public async Task<ActionResult> Index()
        {
            //var allUsers=(List<ADModel>) something;
            try
            {
                helper.GivePermissionToFolder();
                await LoadAllAdUser();
                GetUserInRole();
            }
            catch (Exception ex)
            {
                log.Error("Error " + ex);
            }


            return View();
        }


        public async Task<ActionResult> SubmitConcept()
        {
            var documentInfo = new DocumentInfo();
            try
            {
                string requestId = (string) Session["requestId"];
                string currentPageRequestId = Request.Form["requestId"];
                //Check DocumentInfos for concept Document with the request id
                
                var documentTypeInfos = await productService.CachedDocumentTypeInfoList();

                var documentTypeInfo = documentTypeInfos.SingleOrDefault(x => x.DocumentType == "Concept");
                log.Info("requestId in session " + requestId);
                log.Info("currentPageRequestId is " + currentPageRequestId);
                HttpResponseMessage getResponse = await productService.GetDocumentInfoListResponseByRequestId(requestId);

                if (getResponse.StatusCode == HttpStatusCode.OK)
                {

                    //Check if the document with Concept exists in the documents submitted by this request Id
                    var documentInfoList = getResponse.Content.ReadAsAsync<List<DocumentInfo>>().Result;

                    documentInfo =
                        documentInfoList.SingleOrDefault(
                            x => documentTypeInfo != null && x.DocumentTypeId == documentTypeInfo.Id);
                    if (documentInfo != null)
                    {
                        log.Info("DocumentInfo : " + documentInfo.DocumentName);
                        var conceptInfo = await productService.GetConceptInfoByRequestId(documentInfo.RequestId);
                        SetDataField(conceptInfo);
                        Session.Remove("requestId");
                        return Json(new {s = "Concept Submitted Successfully and workflow initiated"});
                    }
                    else
                    {
                        return
                            Json(
                                new
                                {
                                    f =
                                        "No Concept Document found for this request..Kindly upload the necessary concept document"
                                });
                    }
                
            }
               
            return Json(new {f = "There are no documents associated with the concept request submitted "});
            }
            catch (Exception ex)
            {
                log.Error("Error submitting Concept " + ex);
                return
                    Json(
                        new
                        {
                            f =
                                "Concept Submission Failed"
                        });
            }
        }

        public async Task<ActionResult> CreateConcept(string tag, string conceptName, string description, IEnumerable<string> stakeHolder,
            string conceptOwner, string requestId, bool isNewConcept = false)
        {
            string userId = User.Identity.Name;
            string stakeHolders=string.Empty;
            try
            {
                stakeHolders = stakeHolder.Aggregate(stakeHolders, (current, stakeHolderInfo) => current + (stakeHolderInfo + ";"));
                log.Info("stakeHolders " + stakeHolders);
                Session.Add("tag", tag);
                Session.Add("requestId", requestId);
                var result = new CommandStatus();
                string conceptProvisionUrl = ConfigurationManager.AppSettings["sdpApiUrl"];
                var provisionHelper = new ProvisionHelper(conceptProvisionUrl);
                var args = new Dictionary<string, string>
                {
                    {"userId", userId.Substring(11)},
                           {"msg", tag},
                           {"conceptName",conceptName},
                            {"description",description},
                            {"isNewConcept",false.ToString()},
                            {"conceptOwner",conceptOwner},
                            {"requestId",requestId},
                            {"stakeHolders",stakeHolders}
                };
                result = provisionHelper.ProvisionConcept(args);
                if (result.StatusCode == CommandCode.Ok)
                {
                    //Create stakeHolder

                    return Json(new { s = "Concept has been successfully created..Kindly attach relevant document(s)" });
                }
                return Json(new { f = "Concept Creation failed" });
                
            }
            catch (Exception ex)
            {
                log.Error("Error occured in create concept " + ex);
                return Json(new {f = "Ooops!! something has gone wrong"});
            }
        }

        ProductService productService = new ProductService();
        public void SetDataField(ConceptInfo conceptInfo)
        {
            //string approvalPage = @"http://localhost:8013/WorkList/ShowWorkListItem";
            log.Info(User.Identity.Name);
            using (Connection connection = new Connection())
            {
                try
                {
                    log.Info("I am here");
                    if (conceptInfo != null)
                    {
                        connection.Open("localhost");
                        ProcessInstance processInstance = connection.CreateProcessInstance(@"ProductImplementation\Concept");
                        processInstance.DataFields["conceptName"].Value = conceptInfo.ConceptName;
                        processInstance.Folio = User.Identity.Name.Substring(11) + "-" + conceptInfo.RequestId.ToUpper();
                        processInstance.DataFields["conceptOwner"].Value = conceptInfo.ConceptOwner;
                        processInstance.DataFields["ProductDescription"].Value = conceptInfo.ProductDescription;
                        processInstance.DataFields["RequestId"].Value = conceptInfo.RequestId;
                        processInstance.DataFields["OriginatorUserName"].Value = User.Identity.Name;
                        processInstance.DataFields["Status"].Value = conceptInfo.Status;
                        processInstance.DataFields["CurrentActivityState"].Value = conceptInfo.CurrentActivityState;
                        processInstance.DataFields["OriginatorEmail"].Value = conceptInfo.OriginatorEmail;
                        //processInstance.Folio = "concept " + conceptInfo.RequestId;
                        processInstance.DataFields["Tag"].Value = conceptInfo.Tag;
                        processInstance.DataFields["CreationDate"].Value = DateTime.Now;

                        //if (ReferenceEquals(processInstance.DataFields["SaveForlater"].Value, "SaveForlater"))
                        //{

                        //    //k2MngServer.GotoActivity(processInstance.ID, "Save For Later");
                        //    //k2MngServer.Connection.Close();
                        //}

                        connection.StartProcessInstance(processInstance);
                        log.Info("Process Initiated");


                        //ServerEventContext se;

                    }

                }
                catch (Exception ex)
                {
                    log.Error("Error in Submiting  ConceptRequest " + ex);
                }
            }

        }
        public async Task<List<ADModel>> LoadAllAdUser()
        {
            var aduserList = await productService.GetADUserList();
            var newUserList =
                aduserList.Where(x => x.FirstName != null && x.LastName != null).OrderBy(y => y.FirstName).ToList();

            TempData.Keep("adUserList");
            TempData["aduserList"] = newUserList;
            //var documentInfoList = new List<DocumentInfo>();
            //string requestId = string.Empty;
            var documentTypeList = await productService.CachedDocumentTypeInfoList();
            TempData.Keep("documentTypeList");
            TempData["documentTypeList"] = documentTypeList;
            return aduserList;
        }

        public async Task<ActionResult> Review(string SN)
        {
             //string impersonatedUser = @"K2:TESTPORTAL\mossadmin";
            string impersonatedUser = @"K2:" + User.Identity.Name;
            var conceptInfo=new ConceptInfo();
            var k2ActionList = new List<string>();
            using (var connection = new Connection())
            {
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    conceptInfo = new ConceptInfo
                    {
                        SN = worklistItem.SerialNumber,
                        OriginatorUserName = (string) worklistItem.ProcessInstance.DataFields["OriginatorUserName"].Value,
                        ConceptName = (string)worklistItem.ProcessInstance.DataFields["ConceptName"].Value ,
                        ConceptOwner = (string)worklistItem.ProcessInstance.DataFields["ConceptOwner"].Value,
                        CreationDate = (DateTime) worklistItem.ProcessInstance.DataFields["CreationDate"].Value,
                        CurrentActivityState = (string)worklistItem.ProcessInstance.DataFields["CurrentActivityState"].Value,
                        ProductDescription  = (string)worklistItem.ProcessInstance.DataFields["ProductDescription"].Value,
                        OriginatorEmail  = (string)worklistItem.ProcessInstance.DataFields["OriginatorEmail"].Value,
                        Tag = (string)worklistItem.ProcessInstance.DataFields["Tag"].Value,
                        RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value
                       
                    };
                    var adUserList = await productService.GetADUserList();
                    var adUser = adUserList.SingleOrDefault(x => (string) x.UserName == conceptInfo.OriginatorUserName);
                    if (adUser != null)
                        conceptInfo.OriginatorName = adUser.FirstName+" "+adUser.LastName;
                    log.Info("Full Name of Initiator " + conceptInfo.OriginatorName);
                    TempData.Keep("K2ActionName");
                    foreach (SourceCode.Workflow.Client.Action k2Action in worklistItem.Actions)
                    {
                        k2ActionList.Add(k2Action.Name);

                    }
                    TempData["K2ActionName"] = k2ActionList;
                }
                catch (Exception ex)
                {
                   // new WorkflowHelper().ManageWorklistItems();
;                    log.Error("Error Loading WorkListItem" + ex);
                    return RedirectToAction("PipWorkList");
                }
                connection.Close();
            }
            return View(conceptInfo);

        }
        public async Task<ActionResult> ApproveConceptRequest(string requestId, string ApprovalAction, string ApproverComment, string SN)
        {
            using (var connection = new Connection())
            {
                //string impersonatedUser = @"K2:TESTPORTAL\mossadmin";
                string impersonatedUser = @"K2:" + User.Identity.Name;
                ActionHistoryInfo actionHistoryInfo;
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    //GetConceptInfoById
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    log.Info("Concept Info SN is " + conceptInfo.Id);
                    log.Info("Concept Date is " + conceptInfo.CreationDate);
                    //worklistItem.ProcessInstance.DataFields["ApproverComment"].Value = ApproverComment;
                    log.Info("Action Performed is " + ApprovalAction);
                    var theStartDate = worklistItem.ActivityInstanceDestination.StartDate;
                    //complete the task with the selected action
                    worklistItem.Actions[ApprovalAction].Execute();
                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = conceptInfo.RequestId,
                        Action = ApprovalAction,
                        ActionTimeStamp = DateTime.Now,
                        Comment = ApproverComment,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                       RequesterName = conceptInfo.OriginatorUserName,
                        Participant = User.Identity.Name,
                        WorkListArrivalTime = theStartDate
                       
                    };
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    //If accepted, Create A BRDInfo
                    var adUserList = await productService.GetADUserList();
                    log.Info("BRD ConceptInfo: " + conceptInfo.RequestId);
                    //var adUser = adUserList.SingleOrDefault(x => (string) x.UserName == conceptInfo.OriginatorUserName);
                    if (ApprovalAction == "Accept")
                    {
                        var brdInfo = new BrdInfo
                        {
                            RequestId = conceptInfo.RequestId,
                            ConceptInfoId = conceptInfo.Id,
                            CreatedDate = DateTime.Now,
                           //get Line Manager
                            //OriginatingDepartment = adUser.Department.ToString(),
                            //ConceptOriginatorLineManager = adUser.Manager.ToString()
                        };
                        //log.Info("Line Manager is: " + brdInfo.ConceptOriginatorLineManager);
                        //log.Info("Department is : " + brdInfo.OriginatingDepartment);
                        await new BrdInfosApiController().PostBrdInfo(brdInfo);
                        return Json(new { s = "Concept Submitted has been reviewed and accepted" });
                    }
                    return Json(new { s = "Concept Submitted has been reviewed " });
                }
                catch (Exception ex)
                {
                    log.Error("Error Reviewing concept " + ex);
                    return Json(new { f = "Concept Review Failed" });
                }

            }
            
        }
    }
}