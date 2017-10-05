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

        public async Task<ActionResult> AddNewImplimentationTimeLine(ImplementationTimeline impTimelIne)
        {
            try
            {
                var brdInfo = await productService.GetBrdInfoByRequestId(impTimelIne.RequestId);

                long ImplimentationInfoId = await CreateImplimentationInfo(brdInfo.ConceptInfoId, brdInfo.ConceptInfoId, brdInfo.RequestId, impTimelIne.SN);

                if (ImplimentationInfoId == 0)
                {
                    log.Error("Error in adding new Implimentation timeline. Implimentation Info Creation returns 0 as Id ");
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }
                impTimelIne.ImplementationInfoId = ImplimentationInfoId;

                await new ImplementationTimelinesApiController().PostImplementationTimeline(impTimelIne);

                var newImplTimeline = new List<ImplementationTimeline>();

                newImplTimeline = await productService.GetProductImplTimeListByRequestId(impTimelIne.RequestId);


                var countOfRows = newImplTimeline.Count();

                var lastRow = newImplTimeline.Skip(countOfRows - 1).FirstOrDefault();


                return Json(new { s = "New Implimentation timeline row added!", id = lastRow.Id });
            }
            catch (Exception ex)
            {
                log.Error("Error in adding new Implimentation timeline " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }
        }

        public async Task<ActionResult> RemoveImplimentationTimeLine(long id)
        {
            try
            {
                await new ImplementationTimelinesApiController().DeleteImplementationTimeline(id);
                return Json(new { s = "Row deleted!" });
            }
            catch (Exception ex)
            {
                log.Error("Error in deleting Implimentation timeline " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }
        }

        public async Task<long> CreateImplimentationInfo(long brdInfoId, long conceptId, string requestId, string sn)
        {
            var implementationInfo = new ImplementationInfo();
            try
            {
                implementationInfo = new ImplementationInfo
                {
                    BrdInfoId = brdInfoId,
                    ConceptId = conceptId,
                    RequestId = requestId,
                    SN = sn,
                    Tag = tag
                };


                var newImplimentationInfo = new ImplementationInfo();
                newImplimentationInfo = await productService.GetImplementationInfoByRequestIdandTag(implementationInfo.RequestId, tag);

                if (newImplimentationInfo == null)
                {
                    await new ImplementationInfosApiController().PostImplementationInfo(implementationInfo);
                }

                newImplimentationInfo = await productService.GetImplementationInfoByRequestIdandTag(implementationInfo.RequestId, tag);
                var lastRowId = newImplimentationInfo.Id;

                return lastRowId;
            }
            catch (Exception ex)
            {
                log.Error("Error in adding new Implimentation Info " + ex);
                return 0;
            }
        }

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
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    var requestId = (string) worklistItem.ProcessInstance.DataFields["RequestId"].Value;
                    //febf323c6090
                   // var requestId = "febf323c6090";
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);

                    brdInfo.SN = worklistItem.SerialNumber;
                    //brdInfo.SN = SN;

                    var slaCategoryInfoList = await productService.GetSlaCategoryList();
                    var slaCategoryList = slaCategoryInfoList.Where(x => x.SlaLevel == tag).ToList();
                    
                    TempData.Keep("slaCategoryList");
                    TempData["slaCategoryList"] = slaCategoryList;

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

                    //worklistItem.Actions[approvalAction].Execute();

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

        public async Task<ActionResult> ImplementerDisplay(string SN)
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

                   // WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    //var requestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value;
                    //febf323c6090
                     var requestId = "febf323c6090";
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);

                    //brdInfo.SN = worklistItem.SerialNumber;
                    brdInfo.SN = SN;

                    var activityInfoList = await productService.GetActivityInfoList();
                    TempData.Keep("activityInfoList");
                    TempData["activityInfoList"] = activityInfoList;

                    long roadMapId = (long)brdInfo.RoadMapId;
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


                    //await LoadAllAdUser();

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

        public async Task<ActionResult> ImplApproval()
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            helper.GivePermissionToFolder();
            await LoadAllAdUser();
            var brdInfo = new BrdInfo();
            var k2ActionList = new List<string>();
            using (var connection = new Connection())
            {
                try
                {
                    //connection.Open("localhost");
                    //connection.ImpersonateUser(impersonatedUser);

                    //WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    //string RequestString = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value;

                    //brdInfo = new BrdInfo
                    //{
                    //    SN = worklistItem.SerialNumber,
                    //    ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value,
                    //    RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value,
                    //    CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value,
                    //};


                    var conceptInfo = await productService.GetConceptInfoByRequestId("90a6dc9abae9");
                    var implementationInfo = await productService.GetImplementationInfoByRequestIdandTag(conceptInfo.RequestId, tag);
                    var prodImplTimelineList = await productService.GetImplementationTimeListByImplementationId(implementationInfo.Id);

                    TempData.Keep("implementation");
                    TempData["implementation"] = prodImplTimelineList;

                    TempData.Keep("conceptInfo");
                    TempData["conceptInfo"] = conceptInfo;
                    log.Info("Concept name: " + conceptInfo.ConceptName);
                    //API URL apiUrl
                    TempData.Keep("apiUrl");
                    TempData["apiUrl"] = apiUrl;
                    TempData.Keep("apiUrl");

                    var documentInfoList = await productService.GetDocumentInfoListByRequestId(conceptInfo.RequestId);
                    TempData.Keep("documentInfoList");
                    TempData["documentInfoList"] = documentInfoList;
                    TempData.Keep("K2ActionName");
                    //Get Road Maps
                    var RoadMaps = new List<RoadMapMaster>();
                    RoadMaps = await productService.GetRoadMapMasterInfoList();
                    //Get Current Road Maps
                    var currentRoadMaps = RoadMaps.FindAll(x => x.EndDate > DateTime.Today);

                    TempData.Keep("currentRoadMaps");
                    TempData["currentRoadMaps"] = currentRoadMaps;

                    //
                    var actionHistory = new List<ActionHistoryInfo>();

                    actionHistory = await productService.GetActionHistoryInfoListByRequestId(conceptInfo.RequestId);
                    log.Info(actionHistory.Count);

                    TempData.Keep("actionHistory");
                    TempData["actionHistory"] = actionHistory;



                    //foreach (SourceCode.Workflow.Client.Action k2Action in worklistItem.Actions)
                    //{
                    //    k2ActionList.Add(k2Action.Name);

                    //}
                    //TempData["K2ActionName"] = k2ActionList;
                    List<string> fakeList = new List<string> { "Accept", "Reject" };
                    TempData["K2ActionName"] = fakeList;
                    return View(brdInfo);
                }
                catch (Exception ex)
                {

                    log.Error("Error Loading WorkListItem" + ex);
                    // return RedirectToAction("PipWorkList");
                }

                connection.Close();
                return View();
            }
        }

        public async Task<ActionResult> ActionApproval(string requestId, string action, string SN)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            helper.GivePermissionToFolder();
            await LoadAllAdUser();
            ActionHistoryInfosApiController actionHistorySubProcess = new ActionHistoryInfosApiController();
            var brdInfo = new BrdInfo();
            var actionHistory = new List<ActionHistoryInfo>();
            ActionHistoryInfo logActionHistory = new ActionHistoryInfo();
            
            BrdInfosApiController brdApi = new BrdInfosApiController();
            using (var connection = new Connection())
            {
                RoleHelper userRole = new RoleHelper();
                try
                {
                    var pauseInfoRequest = await productService.GetPauseInfoByRequestIdAndStatus(requestId, "Pending");
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = pauseInfoRequest.PauseOriginator;

                    worklistItem.Actions[action].Execute();

                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);
                    brdInfo.CurrentActivityState = action;
                    brdInfo.Status = "Pause action " + action;
                    await brdApi.PutBrdInfo(brdInfo.Id, brdInfo);

                    //Log Action History
                    logActionHistory = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        Action = action,
                        ActionTimeStamp = DateTime.Now,
                        ProcessName = "PIP",
                        Participant = impersonatedUser,
                        DestinationUser = pauseInfoRequest.PauseOriginator,
                        Activity = "Line Manager Pause Action",
                        SubProcess = "BRD",
                        //RequesterName = lastActionHistory.RequesterName,
                        WorkListArrivalTime = DateTime.Now
                    };
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(logActionHistory);

                    //Log Task Info
                    var taskInfo = new TaskInfo
                    {
                        TaskName = "Line Manager Pause Action",
                        AssignedBy = impersonatedUser,
                        AssignmentDate = DateTime.Now,
                        SN = SN,
                        Tag = tag,
                        Assignee = pauseInfoRequest.PauseOriginator,
                        AssignmentType = "Original",
                        RequestId = requestId
                    };
                    await new TaskInfosApiController().PostTaskInfo(taskInfo);

                    if (action.Trim() == "Approve")
                    {
                        var pauseInfo = new PauseInfo
                        {
                            RequestId = requestId,
                            Actor = impersonatedUser,
                            DatePaused = DateTime.Now,
                            Status = "Paused"
                        };

                        await new PauseInfosApiController().PostPauseInfo(pauseInfo);
                    }




                    return Json(new { s = "Task action assigned Successfully" });

                }
                catch (Exception ex)
                {

                    log.Info("the error is" + ex);
                }
            }
            return Json(new { f = "There are no documents associated with the brd request submitted " });

        }

    }
}
