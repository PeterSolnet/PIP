using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using K2.WebApi.Controllers;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using PiPWeb.Helper;
using PiPWeb.Services;
using SourceCode.Workflow.Client;

namespace PiPWeb.Controllers
{
    public class ImplementationController : Controller
    {

        // GET: /Implementation/

        protected static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        ProductService productService = new ProductService();

        string tag = ConfigurationManager.AppSettings["ProdImplTAG"];
        string brdTag = ConfigurationManager.AppSettings["BRDTAG"]; 
        DocumentHelper helper = new DocumentHelper();
        string apiUrl = ConfigurationManager.AppSettings["apiUrl"];



        [Authorize]
        public async Task<ActionResult> Index(string SN)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            var brdInfo = new BrdInfo();
            var k2ActionList = new List<string>();
            using (var connection = new Connection())
            {
                try
                {
                    //connection.Open("localhost");
                    //connection.ImpersonateUser(impersonatedUser);

                    //WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    //var requestId = (string) worklistItem.ProcessInstance.DataFields["RequestId"].Value;
                    //febf323c6090
                    var requestId = "febf323c6090";
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);

                    //brdInfo.SN = worklistItem.SerialNumber;
                    brdInfo.SN = SN;

                    var slaCategoryInfoList = await productService.GetSlaCategoryList();
                    //var slaCategoryList = slaCategoryInfoList.Where(x => x.SlaLevel == tag).ToList();
                    
                    TempData.Keep("slaCategoryList");
                    TempData["slaCategoryList"] = slaCategoryInfoList;

                    long roadMapId = (long) brdInfo.RoadMapId;
                    RoadMap roadMapInfo = await productService.GetRoadMapInfoById(roadMapId);
                    RoadMapMaster roadMapMasterInfo = await productService.GetRoadMapMasterInfoByRoadMapMasterId(roadMapInfo.RoadMapMasterId);

                    TempData.Keep("roadMapInfo");
                    TempData["roadMapInfo"] = roadMapInfo;

                    TempData.Keep("roadMapMasterInfo");
                    TempData["roadMapMasterInfo"] = roadMapMasterInfo;


                    var stakeHolders = await productService.GetStakeHolderInfoById(brdInfo.Stakeholder);
                    TempData.Keep("stakeHolders");
                    TempData["stakeHolders"] = stakeHolders;

                    //BRD Stage
                    var implimenatationInfoBrd = await productService.GetImplementationInfoByRequestIdandTag(brdInfo.RequestId, brdTag);
                    if (implimenatationInfoBrd != null)
                    {
                        var BrdprodImplTimelineList = await productService.GetImplementationTimeListByImplementationId(implimenatationInfoBrd.Id);
                        TempData.Keep("BrdprodImplTimelineList");
                        TempData["BrdprodImplTimelineList"] = BrdprodImplTimelineList;
                    }

                    //Implementation Stage
                    var implimenatationInfo = await productService.GetImplementationInfoByRequestIdandTag(brdInfo.RequestId, tag);

                    if (implimenatationInfo != null)
                    {
                        var prodImplTimelineList = await productService.GetImplementationTimeListByImplementationId(implimenatationInfo.Id);
                        TempData.Keep("prodImplTimelineList");
                        TempData["prodImplTimelineList"] = prodImplTimelineList;
                    }

                    var actionHistory = new List<ActionHistoryInfo>();

                    actionHistory = await productService.GetActionHistoryInfoListByRequestId(brdInfo.RequestId);

                    TempData.Keep("actionHistory");
                    TempData["actionHistory"] = actionHistory;

                    var documentInfoList = await productService.GetDocumentInfoListByRequestId(brdInfo.RequestId);
                    TempData.Keep("documentInfoList");
                    TempData["documentInfoList"] = documentInfoList;
                    

                    await LoadAllAdUser();
                    
                    TempData.Keep("K2ActionName");
                    //foreach (SourceCode.Workflow.Client.Action k2Action in worklistItem.Actions)
                    //{
                    //    k2ActionList.Add(k2Action.Name);
                    //}
                    k2ActionList.Add("Reject");
                    k2ActionList.Add("Assign");
                    TempData["K2ActionName"] = k2ActionList;
                }
                catch (Exception ex)
                {
                     log.Error("Error Loading Implementation Stage WorkListItem to Approve BRD. " + ex);
                    
                }
                connection.Close();
            }
            return View(brdInfo);
            
        }

        public async Task<ActionResult> ApproveBrdRequest(string SN, string requestId, string approvalAction, string taskAsssignedTo, string approverComment, string taskPriority, string slaCategory)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
           
            var brdInfo = new BrdInfo();
            var actionHistory = new List<ActionHistoryInfo>();
            ActionHistoryInfo logActionHistory = new ActionHistoryInfo();

           

            BrdInfosApiController brdApi = new BrdInfosApiController();
            using (var connection = new Connection())
            {
                RoleHelper userRole = new RoleHelper();
                try
                {
                    //Log Action History
                    
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = impersonatedUser;
                     
                     brdInfo = await productService.GetBrdInfoByRequestId(requestId);

                    var implementationInfo = new ImplementationInfo
                    {
                        RequestId = requestId,
                        DateCreated = DateTime.Now,
                        Tag = tag,
                        ConceptId = brdInfo.ConceptInfoId,
                        BrdInfoId = brdInfo.Id
                    };

                    
                     
                     logActionHistory = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        Action = approvalAction,
                        ActionTimeStamp = DateTime.Now,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        Participant = impersonatedUser,
                        DestinationUser = taskAsssignedTo,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        SubProcess = "Implementation",
                        WorkListArrivalTime = worklistItem.ActivityInstanceDestination.StartDate
                    };

                    //Task assignment

                    var taskAssignment = new TaskInfo
                    {
                        TaskName = worklistItem.ActivityInstanceDestination.Name,
                        Assignee = taskAsssignedTo,
                        AssignedBy = impersonatedUser,
                        AssignmentDate = DateTime.Now,
                        AssignmentType = "Original",
                        TaskPriority = taskPriority,
                        RequestId = requestId,
                        Tag = tag,
                        SN = SN
                    };
                    await new TaskInfosApiController().PostTaskInfo(taskAssignment);
                    await new ImplementationInfosApiController().PostImplementationInfo(implementationInfo);
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(logActionHistory);
                     return Json(new { s = "Task "+ approvalAction + " Successfully" });
                       
                    
                }
                catch (Exception ex)
                {

                    log.Info("Error encountered in assigning Task by Line Manager on Product Implementation Stage" + ex);
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }
                connection.Close();
            }
            
        }


        public async Task<ActionResult> RejectBrdRequest(string SN, string requestId, string approvalAction, string approverComment)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            var actionHistory = new List<ActionHistoryInfo>();
            ActionHistoryInfo logActionHistory = new ActionHistoryInfo();

            BrdInfosApiController brdApi = new BrdInfosApiController();
            using (var connection = new Connection())
            {
                RoleHelper userRole = new RoleHelper();
                try
                {
                    //Log Action History
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = conceptInfo.OriginatorUserName;
                   
                    logActionHistory = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        Action = approvalAction,
                        ActionTimeStamp = DateTime.Now,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        Participant = impersonatedUser,
                        DestinationUser = impersonatedUser,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        SubProcess = "Implementation",
                        WorkListArrivalTime = worklistItem.ActivityInstanceDestination.StartDate
                    };

                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(logActionHistory);
                    //worklistItem.Actions[approvalAction].Execute();
                    return Json(new { s = "Task " + approvalAction + " Successfully" });
                }
                catch (Exception ex)
                {
                    log.Info("Error encountered in Rejecting BRD by Line Manager on Product Implementation Stage" + ex);
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }
                connection.Close();
            }

        }

        public async Task<List<ADModel>> LoadAllAdUser()
        {
            var aduserList = await productService.GetADUserList();
            var newUserList =
                aduserList.Where(x => x.FirstName != null && x.LastName != null).OrderBy(y => y.FirstName).ToList();

            TempData.Keep("adUserList");
            TempData["aduserList"] = newUserList;

            var documentTypeList = await productService.CachedDocumentTypeInfoList();
            TempData.Keep("documentTypeList");
            TempData["documentTypeList"] = documentTypeList;
            return aduserList;
        }


        

    }
}
