using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using K2.WebApi.Controllers;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using Newtonsoft.Json;
using PiPWeb.Helper;
using PiPWeb.Services;
using SourceCode.Workflow.Client;

namespace PiPWeb.Controllers
{
    public class ProductController : Controller
    {


        // GET: /Product/
        protected static readonly ILog log = LogManager.GetLogger(typeof(ProductController));
        
        StakeHoldersApiController stakeHoldersApi = new StakeHoldersApiController();
        
        TaskInfosApiController taskInfosApi = new TaskInfosApiController();
        
        ConceptInfosApiController conceptInfosApi = new ConceptInfosApiController();
        
        BrdInfosApiController brdInfosApi = new BrdInfosApiController();
        
        ActionHistoryInfosApiController actionHistoryInfosApi = new ActionHistoryInfosApiController();
        
        ProductService productService = new ProductService();


        string tag = ConfigurationManager.AppSettings["BRDTAG"]; 
        string ImplementationRoleMgr = ConfigurationManager.AppSettings["ImplementationRoleMgr"];
        DocumentHelper helper = new DocumentHelper();
        string apiUrl = ConfigurationManager.AppSettings["apiUrl"];
        string brdRoleMgr = ConfigurationManager.AppSettings["BRDroleMgr"];

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
                //await productService.DeleteImplementationTimeline(id);
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
                    Tag=tag
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
            TempData.Keep("user");
            TempData["user"] = impersonatedUser;
            helper.GivePermissionToFolder();
            var adUserList = await productService.GetADUserList();
            TempData.Keep("aduserList");
            TempData["aduserList"] = adUserList;

            var activityInfoList = await productService.GetActivityInfoList();
            TempData.Keep("activityInfoList");
            TempData["activityInfoList"] = activityInfoList;

            var brdInfo = new BrdInfo();
            var k2ActionList = new List<string>();
            using (var connection = new Connection())
            {
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    brdInfo = new BrdInfo
                    {
                        SN = worklistItem.SerialNumber,
                        ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value,
                        RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value,
                        CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value,
                    };
                   

                    var conceptInfo = await productService.GetConceptInfoByRequestId(brdInfo.RequestId);
                    TempData.Keep("conceptInfo");
                    TempData["conceptInfo"] = conceptInfo;

                    var documentTypeList = await productService.CachedDocumentTypeInfoList();
                    TempData.Keep("documentTypeList");
                    TempData["documentTypeList"] = documentTypeList;

                    Session["tag"] = tag;
                    Session["requestId"] = brdInfo.RequestId;

                    var implimenatationInfo = await new ImplementationInfosApiController().GetImplementationInfoByRequestIdandTag(brdInfo.RequestId, tag);
                    if (implimenatationInfo != null)
                    {
                        var prodImplTimelineList = await productService.GetImplementationTimeListByImplementationId(implimenatationInfo.Id);
                        TempData.Keep("prodImplTimelineList");
                        TempData["prodImplTimelineList"] = prodImplTimelineList;
                    }


                    //API URL apiUrl
                    TempData.Keep("apiUrl");
                    TempData["apiUrl"] = apiUrl;
                    TempData.Keep("apiUrl");

                    var documentInfoList = await productService.GetDocumentInfoListByRequestId(conceptInfo.RequestId);
                    TempData.Keep("documentInfoList");
                    TempData["documentInfoList"] = documentInfoList;
                    TempData.Keep("K2ActionName");
                   

                    await GetRoadMapList();

                    var actionHistory = new List<ActionHistoryInfo>();

                    actionHistory = await productService.GetActionHistoryInfoListByRequestId(brdInfo.RequestId);

                    TempData.Keep("actionHistory");
                    TempData["actionHistory"] = actionHistory;

                    var taskInfoList = await productService.GetTaskInfoListByRequestId(brdInfo.RequestId);

                    TempData.Keep("taskInfo");
                    TempData["taskInfo"] =
                        taskInfoList.FindLast(x => x.Assignee == impersonatedUser && x.Tag == tag);
                }
                catch (Exception ex)
                {
                    log.Error("Error Loading Page " + ex);
                    // return RedirectToAction("PipWorkList");
                }
                connection.Close();
            }
           

            return View(brdInfo);
        }

        public async Task<List<RoadMapViewModel>> GetRoadMapList()
        {
            var roadMapList = await productService.GetRoadMapList();
            var roadMapMaster = await productService.GetRoadMapMasterInfoList();

            List<RoadMapViewModel> roadMapLists = (from pd in roadMapList
                                                   join p in roadMapMaster on pd.RoadMapMasterId equals p.Id
                                                   orderby pd.Id
                                                   select new RoadMapViewModel
                                                   {
                                                       Id = pd.Id,
                                                       RoadMapMasterId = pd.RoadMapMasterId,
                                                       RoadMapName = p.RoadMapName,
                                                       ConceptName = pd.ConceptName,
                                                       ConceptOwner = pd.ConceptOwner
                                                   }).ToList();

            TempData.Keep("currentRoadMaps");
            TempData["currentRoadMaps"] = roadMapLists;
            return roadMapLists;
        }
       
        public async Task<ActionResult> SubmitDraftBrd(long conceptInfo, string requestId, long roadmap, string reviewStartDate, string SN,
            IEnumerable<string> internalRev, string externalRev, string status)
        {

            using (var connection = new Connection())
            {

                try
                {
                    //Confirm Draft BRD is Submitted
                    connection.Open("localhost");
                    string impersonatedUser = @"K2:" + User.Identity.Name;
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = impersonatedUser;
                    var documentTypeInfos = await productService.CachedDocumentTypeInfoList();

                    string draftBrdDoc = ConfigurationManager.AppSettings["DraftBRD"];

                    var draftdoc = documentTypeInfos.SingleOrDefault(x => x.DocumentType == draftBrdDoc);
                    if (draftdoc == null)
                    {
                        return Json(new { f = "No Document type '" + draftdoc + "' available in Records" });
                    }


                    var allDocs = await productService.GetDocumentInfoListByRequestId(requestId);

                    var docAvailable = allDocs.SingleOrDefault(x => x.DocumentTypeId == draftdoc.Id);
                    if (docAvailable == null)
                    {
                        return Json(new { f = "You must upload 'Draft BRD' to continue!" });
                    }

                    //Confirm Implimentation Timeline Activities has been uploaded.

                    var implimTimelines = await new ImplementationTimelinesApiController().GetImplTimelinesListRequestId(requestId);

                    if (implimTimelines == null)
                    {
                        return Json(new { f = "No Implimentation timeline activity found! You must add implimentation activities and timelines to proceed." });
                    }


                    var conceptDetails = await productService.GetConceptInfoByRequestId(requestId);

                    

                    //Log Stakeholders
                    string stakeHolders = internalRev.Aggregate("", (current, item) => current + (item.ToString() + ";"));
                    var stakeholder = new StakeHolder
                    {
                        StakeHolderName = stakeHolders,
                        RequestId = requestId,
                        Tag = tag
                    };
                    await new StakeHoldersApiController().PostStakeHolder(stakeholder);

                    log.Info("Post Stakeholders");
                    //Log Action History

                    //Get Last WorkList Arrival Time
                   // var currentActionHistoryList = await new ActionHistoryInfosApiController().GetActionHistoryInfoListByRequestId(requestId);
                    var arrivalTime = worklistItem.ActivityInstanceDestination.StartDate;

                    var actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        ActionTimeStamp = DateTime.Now,
                        WorkListArrivalTime = arrivalTime,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        Participant = impersonatedUser,
                        DestinationUser = impersonatedUser,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        
                        Action = "Accept",
                        RequesterName = conceptDetails.OriginatorName
                    };


                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);

                    log.Info("Post Action History");

                    RoleHelper userRole = new RoleHelper();

                    var brdInfo = await productService.GetBrdInfoByRequestId(requestId);


                    brdInfo.ConceptOriginatorLineManager = userRole.GetUserInRole(brdRoleMgr).Trim();
                    // brdInfo.ConceptInfoId = conceptDetails.Id;
                    brdInfo.SN = SN;
                    //brdInfo.CreatedDate = DateTime.Now;
                    brdInfo.CurrentActivityState = "BRD Drafting";
                    brdInfo.ExternalReviewer = externalRev;
                    brdInfo.ReviewMeetingDate = DateTime.Parse(reviewStartDate);
                    brdInfo.RoadMapId = roadmap;
                    brdInfo.Stakeholder = stakeholder.Id;
                    brdInfo.Status = "BRD Draft Submitted";
                    //brdInfo.RequestId = requestId;


                    await new BrdInfosApiController().PutBrdInfo(brdInfo.Id, brdInfo);

                    log.Info("Post BRD Info");

                    //Insert Task Info

                    var taskInfo = new TaskInfo
                    {
                        TaskName = "BRD Drafting",
                        Assignee = impersonatedUser,
                        AssignedBy = impersonatedUser,
                        AssignmentDate = DateTime.Now,
                        AssignmentType = "Original",
                        //TaskPriority = "",
                        RequestId = requestId,
                        SN = SN,
                        Tag = tag
                    };


                    log.Info("Post TaskInfo");


                    worklistItem.Actions["Accept"].Execute();
                    log.Info("Actioned to Workflow");
                }
                catch (Exception ex)
                {
                    log.Error("Error occured in BRD Drafting task fulfill " + ex);
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }

            }
            return Json(new { s = "BRD Drafting Submitted Successfully!" });
        }
        
        [Authorize]
        public async Task<ActionResult> BRDStatusUpdate(string SN)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            helper.GivePermissionToFolder();
            var adUserList = await productService.GetADUserList();
            TempData.Keep("aduserList");
            TempData["aduserList"] = adUserList;
            BrdInfo brdInfo = new BrdInfo();
            var k2ActionList = new List<string>();

            var documentTypeList = await productService.CachedDocumentTypeInfoList();
            TempData.Keep("documentTypeList");
            TempData["documentTypeList"] = documentTypeList;

            using (var connection = new Connection())
            {
                try
                {

                    var activityInfoList = await productService.GetActivityInfoList();
                    TempData.Keep("activityInfoList");
                    TempData["activityInfoList"] = activityInfoList;

                    TempData.Keep("apiUrl");
                    TempData["apiUrl"] = apiUrl;
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    brdInfo = new BrdInfo
                    {
                        SN = worklistItem.SerialNumber,
                        ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value,
                        RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value,
                        CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value,
                    };

                    brdInfo = await productService.GetBrdInfoByRequestId(brdInfo.RequestId);

                    brdInfo.SN = worklistItem.SerialNumber;
                    brdInfo.ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value;
                    brdInfo.RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value;
                    brdInfo.CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value;

                    var conceptInfo = await productService.GetConceptInfoByRequestId(brdInfo.RequestId);
                    //var conceptInfo = await productService.GetConceptInfoByRequestId("895aebf81be2");

                    //brdInfo = await productService.GetBrdInfoByRequestId(conceptInfo.RequestId);

                    TempData.Keep("conceptInfo");//
                    TempData["conceptInfo"] = conceptInfo;

                    TempData.Keep("brdInfo");
                    TempData["brdInfo"] = brdInfo;
                    //taskInfoList
                    var taskInfoList = await taskInfosApi.GetTaskInfoListByRequestId(conceptInfo.RequestId);
                    TempData.Keep("taskInfoList");
                    TempData["taskInfoList"] = taskInfoList;

                    long roadMapId = (long)brdInfo.RoadMapId;

                    RoadMap roadMapInfo = await productService.GetRoadMapInfoById(roadMapId);
                    RoadMapMaster roadMapMasterInfo = await productService.GetRoadMapMasterInfoByRoadMapMasterId(roadMapInfo.RoadMapMasterId);

                    TempData.Keep("roadMapInfo");
                    TempData["roadMapInfo"] = roadMapInfo;

                    TempData.Keep("roadMapMasterInfo");
                    TempData["roadMapMasterInfo"] = roadMapMasterInfo;

                    var documentInfoList = await productService.GetDocumentInfoListByRequestId(conceptInfo.RequestId);
                    TempData.Keep("documentInfoList");
                    TempData["documentInfoList"] = documentInfoList;
                    Session["tag"] = tag;
                    Session["requestId"] = brdInfo.RequestId;

                    var stakeHolders = await productService.GetStakeHolderInfoById(brdInfo.Stakeholder);
                    TempData.Keep("stakeHolders");
                    TempData["stakeHolders"] = stakeHolders;

                    var actionHistory = new List<ActionHistoryInfo>();

                    actionHistory = await productService.GetActionHistoryInfoListByRequestId(conceptInfo.RequestId);
                    TempData.Keep("actionHistory");
                    TempData["actionHistory"] = actionHistory;

                    //var prodImplTimelineList = new List<ImplementationTimeline>();
                    var proImpInfo = new ImplementationInfo();

                    var implementationInfo = await productService.GetImplementationInfoByRequestIdandTag(conceptInfo.RequestId, tag);

                    var prodImplTimelineList = await productService.GetImplementationTimeListByImplementationId(implementationInfo.Id);


                    TempData.Keep("prodImplTimeline");
                    TempData["prodImplTimeline"] = prodImplTimelineList;


                    foreach (SourceCode.Workflow.Client.Action k2Action in worklistItem.Actions)
                    {
                        k2ActionList.Add(k2Action.Name);

                    }
                    TempData["K2ActionName"] = k2ActionList;
                    
                }
                catch (Exception ex)
                {
                    
                    log.Error("Error" + ex);
                    
                }
                connection.Close();
            }
          

            return View(brdInfo);

            // return View();
        }

        public async Task<ActionResult> PauseActionApproval(string requestId, string action, string SN)
        {
            string impersonatedUser = @"K2:" + User.Identity.Name;
            helper.GivePermissionToFolder();
            await LoadAllAdUser();
            ActionHistoryInfosApiController actionHistorySubProcess = new ActionHistoryInfosApiController();
            var brdInfo = new BrdInfo();
            var actionHistory = new List<ActionHistoryInfo>();
            ActionHistoryInfo logActionHistory = new ActionHistoryInfo();

            //Get the guy that submitted the pause
            var pauseInfo = await productService.GetPauseInfoInfoByRequestId(requestId, "Pending");
           
            BrdInfosApiController brdApi = new BrdInfosApiController();
            using (var connection = new Connection())
            {
                RoleHelper userRole = new RoleHelper();
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value =pauseInfo.PauseOriginator;

                   

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
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        Participant = impersonatedUser,
                        DestinationUser = pauseInfo.PauseOriginator,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        SubProcess = "BRD",
                        RequesterName = pauseInfo.PauseOriginator,
                        WorkListArrivalTime = worklistItem.ActivityInstanceDestination.StartDate
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
                        Assignee = pauseInfo.PauseOriginator,
                        //AssignmentType = "Original",
                        RequestId = requestId
                    };
                    await new TaskInfosApiController().PostTaskInfo(taskInfo);

                    if (action.Trim() == "Approve")
                    {
                        pauseInfo.Actor = impersonatedUser;
                        pauseInfo.DatePaused=DateTime.Now;
                        pauseInfo.Status = "Paused";
                        pauseInfo.Tag = tag;
                        await new PauseInfosApiController().PutPauseInfo(pauseInfo.Id,pauseInfo);
                        worklistItem.Actions[action].Execute();
                    }


                    return Json(new { s = "Task actioned Successfully" });

                }
                catch (Exception ex)
                {

                    log.Info("the error is" + ex);
                }
                connection.Close();
            }
            return Json(new { f = "There are no documents associated with the brd request submitted " });

        }

        public async Task<ActionResult> RequestPause(string SN, string requestId, string ApproverComment)
        {
            using (var connection = new Connection())
            {
                string impersonatedUser = @"K2:" + User.Identity.Name;
                var actionHistoryInfo = new ActionHistoryInfo();
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    //GetConceptInfoById
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                   
                    RoleHelper userRole = new RoleHelper();
                
                    var pauseInfo = new PauseInfo
                    {
                        RequestId = requestId,
                        PauseOriginator = impersonatedUser,
                        Tag=tag,
                        Status = "Pending",
                        DatePaused = DateTime.Now.AddYears(-17),
                        DateResumed = DateTime.Now.AddYears(-17)
                    };
                    
                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = conceptInfo.RequestId,
                        Action = "Pause Request",
                        ActionTimeStamp = DateTime.Now,
                        Comment = ApproverComment,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        Participant = impersonatedUser,
                        //SubProcess = "BRD",
                        DestinationUser = userRole.GetUserInRole(brdRoleMgr).Trim(),
                        RequesterName = conceptInfo.OriginatorUserName,
                        WorkListArrivalTime = worklistItem.ActivityInstanceDestination.StartDate
                    };

                    var brdinfo = await productService.GetBrdInfoByRequestId(conceptInfo.RequestId);
                    brdinfo.Status = "Pending";

                    await new BrdInfosApiController().PutBrdInfo(brdinfo.Id, brdinfo);
                    
                    await new PauseInfosApiController().PostPauseInfo(pauseInfo);

                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    
                    worklistItem.Actions["Pause"].Execute();
                    return Json(new { s = "BRD Drafting Paused Successfully" });

                }
                catch (Exception ex)
                {
                    log.Error("Error occured in BRD Drafting task pause " + ex);
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }

            }

        }

        public async Task<ActionResult> ReassignDraftBrd(string requestId, string assignedTo, string SN)
        {
            using (var connection = new Connection())
            {

                string impersonatedUser = @"K2:" + User.Identity.Name;
                ActionHistoryInfo actionHistoryInfo;
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    //GetConceptInfoById
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    //worklistItem.ProcessInstance.DataFields["ApproverComment"].Value = ApproverComment;
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = assignedTo;
                    worklistItem.Actions["Reassign"].Execute();
                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = conceptInfo.RequestId,
                        Action = "Reassign Task",
                        ActionTimeStamp = DateTime.Now,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        Participant = impersonatedUser,
                        DestinationUser = assignedTo,
                        Activity = worklistItem.ProcessInstance.Name,
                        SubProcess = "BRD",
                        RequesterName = conceptInfo.OriginatorUserName,
                        WorkListArrivalTime = DateTime.Now
                    };
                    var taskInfo = new TaskInfo
                    {
                        TaskName = "Draf BRD reasign",
                        AssignedBy = impersonatedUser,
                        AssignmentDate = DateTime.Now,
                        SN = SN,
                        Tag = tag,
                        Assignee = assignedTo,
                        AssignmentType = "reassign",
                        RequestId = requestId
                    };

                    var brdInfo = new BrdInfo();
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);
                    brdInfo.CurrentActivityState = "Task Reassigned";
                    
                    await new BrdInfosApiController().PutBrdInfo(brdInfo.Id, brdInfo);
                    await new TaskInfosApiController().PostTaskInfo(taskInfo);
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    //If accepted, Create A BRDInfo


                    //var adUser = adUserList.SingleOrDefault(x => (string) x.UserName == conceptInfo.OriginatorUserName);

                    return Json(new { s = "Task Reassigned Successfully" });

                }
                catch (Exception ex)
                {
                    log.Error("Error occured in BRD Drafting task reassignment " + ex);
                    return Json(new { f = "Ooops!! something has gone wrong" });
                }

            }
        }

        public async Task<ActionResult> AssignTask(string requestId, string SN, string taskPriority, long slaCategory, string assignedTo)
        {
            using (var connection = new Connection())
            {
               
                string impersonatedUser = @"K2:" + User.Identity.Name;
                ActionHistoryInfo actionHistoryInfo;
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    //GetConceptInfoById
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = assignedTo;
                    var activityName=worklistItem.ActivityInstanceDestination.Name;
                    var theStartDate = worklistItem.ActivityInstanceDestination.StartDate;
                   log.Info("the event StartDate " + theStartDate);
                   
                    //worklistItem.ProcessInstance.Update();
                    worklistItem.Actions["Assign"].Execute();
                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = conceptInfo.RequestId,
                        Action = "Assigned Task",
                        DestinationUser = assignedTo,
                        Participant = User.Identity.Name,
                        ActionTimeStamp = DateTime.Now,
                        Activity = activityName,
                        //Comment = ApproverComment,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        RequesterName = conceptInfo.OriginatorUserName,
                        WorkListArrivalTime = theStartDate
                        
                    };
                    var brdInfo = await productService.GetBrdInfoByRequestId(requestId);
                    brdInfo.SlaId = slaCategory;
                    await new BrdInfosApiController().PutBrdInfo(brdInfo.Id, brdInfo);
                    var taskInfo = new TaskInfo
                    {
                        TaskName = "Assign",
                        AssignedBy = impersonatedUser,
                        AssignmentDate = DateTime.Now,
                        SN=SN,
                        Assignee = assignedTo,
                        TaskPriority = taskPriority,
                        Tag =tag ,
                        AssignmentType = "Original",
                        RequestId = requestId
                    };
                    await new TaskInfosApiController().PostTaskInfo(taskInfo);
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    //If accepted, Create A BRDInfo
                  
                    log.Info("Task assigned");
                    //var adUser = adUserList.SingleOrDefault(x => (string) x.UserName == conceptInfo.OriginatorUserName);
                    
                    return Json(new { s = "Task Assigned Successfully" });
                }
                catch (Exception ex)
                {
                    log.Error("Error in Assigning task " + ex);
                    return Json(new { f = "task Assignment Failed" });
                }

            }
        }

        [Authorize]
        public async Task<ActionResult> Manager(string SN)
        {
            var taskInfo=new TaskInfo();
            var brdInfo=new BrdInfo();
            string impersonatedUser = @"K2:" + User.Identity.Name;
            var k2ActionList = new List<string>();
            await LoadAllAdUser();
            
            using (var connection = new Connection())
            {
                try
                {
                    var slaCategoryInfoList = await productService.GetSlaCategoryList();
                    var slaCategoryList = slaCategoryInfoList.Where(x => x.SlaLevel == tag).ToList();
                    
                    TempData.Keep("slaCategoryList");
                    TempData["slaCategoryList"] = slaCategoryList;
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    //var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    brdInfo = new BrdInfo
                    {
                        SN = worklistItem.SerialNumber,
                        ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value,
                        RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value,
                        CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value,
                        
                    };
                   
                    var conceptInfo = await productService.GetConceptInfoByRequestId(brdInfo.RequestId);
                    TempData.Keep("conceptInfo");
                    TempData["conceptInfo"] = conceptInfo;
                    
                    return View(brdInfo);
                }
                catch (Exception ex)
                {
                    // new WorkflowHelper().ManageWorklistItems();
                    log.Error("Error Loading WorkListItem" + ex);
                    // return RedirectToAction("PipWorkList");
                }
                connection.Close();
            }

            return View(brdInfo);
        }

       [Authorize]
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

        [Authorize]
        public async Task<ActionResult> UpdateStatus(string requestId, string SN, string Status, string Comment)
        {

            try
            {
                RoleHelper userRole = new RoleHelper();
                var destinationUser = userRole.GetUserInRole(ImplementationRoleMgr);
                using (var connection = new Connection())
                {

                    string impersonatedUser = @"K2:" + User.Identity.Name;

                    ActionHistoryInfo actionHistoryInfo;

                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = destinationUser;
                    var theStartDate = worklistItem.ActivityInstanceDestination.StartDate;
                    worklistItem.Actions["Baselined"].Execute();
                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        Action = "Basedline BRD Submitted",
                        ActionTimeStamp = DateTime.Now,
                        Comment = Comment,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        RequesterName = conceptInfo.OriginatorUserName,
                        SubProcess = "BRD",
                        DestinationUser = destinationUser,
                        Participant = impersonatedUser,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        WorkListArrivalTime = theStartDate

                    };
                   
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    var brdInfo = new BrdInfo();
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);
                    brdInfo.Status = "BRD Basedlined";
                    brdInfo.CurrentActivityState = "BRD Update";
                    await new BrdInfosApiController().PutBrdInfo(brdInfo.Id, brdInfo);

                    connection.Close();
                    return Json(new { s = "Basedline BRD Submitted Successfully" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { f = "Error in BRD Status Update. " + ex.Message.ToString() });
            }
        }

        public async Task<ActionResult> BrdApproval(string SN)
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
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    string RequestString = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value;

                    brdInfo = new BrdInfo
                    {
                        SN = worklistItem.SerialNumber,
                        ConceptInfoId = (long)worklistItem.ProcessInstance.DataFields["ConceptInfo"].Value,
                        RequestId = (string)worklistItem.ProcessInstance.DataFields["RequestId"].Value,
                        CreatedDate = (DateTime)worklistItem.ProcessInstance.DataFields["CreatedDate"].Value,
                    };


                    var conceptInfo = await productService.GetConceptInfoByRequestId(RequestString);
                    log.Info("The request id from concept object is: " + conceptInfo.RequestId);
                    //var conceptInfo = await productService.GetConceptInfoByRequestId("895aebf81be2");

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
                    var actionHistoryInfoList = new List<ActionHistoryInfo>();

                    actionHistoryInfoList = await productService.GetActionHistoryInfoListByRequestId(conceptInfo.RequestId);
                    log.Info(actionHistoryInfoList.Count);

                    TempData.Keep("actionHistory");
                    TempData["actionHistory"] = actionHistoryInfoList;


                    foreach (SourceCode.Workflow.Client.Action k2Action in worklistItem.Actions)
                    {
                        k2ActionList.Add(k2Action.Name);

                    }
                    TempData["K2ActionName"] = k2ActionList;
                    return View(brdInfo);
                }
                catch (Exception ex)
                {

                    log.Error("Error Loading brd Pause Approval WorkListItem " + ex);
                    // return RedirectToAction("PipWorkList");
                }

                connection.Close();
                return View(brdInfo);
            }
        }

        public async Task<ActionResult> ReturnToConcept(string requestId, string comment, string SN)
        {
            try
            {
                using (var connection = new Connection())
                {

                    string impersonatedUser = @"K2:" + User.Identity.Name;

                    ActionHistoryInfo actionHistoryInfo;

                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                   
                    var conceptInfo = await productService.GetConceptInfoByRequestId(requestId);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = conceptInfo.OriginatorUserName;
                    var theStartDate = worklistItem.ActivityInstanceDestination.StartDate;
                    worklistItem.Actions["Back to Concept"].Execute();

                    actionHistoryInfo = new ActionHistoryInfo
                    {
                        RequestId = requestId,
                        Action = "Returned For Correction",
                        ActionTimeStamp = DateTime.Now,
                        Comment = comment,
                        ProcessName = worklistItem.ProcessInstance.FullName,
                        RequesterName = conceptInfo.OriginatorUserName,
                        SubProcess = "BRD",
                        DestinationUser = conceptInfo.OriginatorUserName,
                        Participant = impersonatedUser,
                        Activity = worklistItem.ActivityInstanceDestination.Name,
                        WorkListArrivalTime = theStartDate

                    };
                    
                    await new ActionHistoryInfosApiController().PostActionHistoryInfo(actionHistoryInfo);
                    var brdInfo = new BrdInfo();
                    brdInfo = await productService.GetBrdInfoByRequestId(requestId);
                    brdInfo.Status = "Back to Concept";
                    brdInfo.CurrentActivityState = "Returned For Correction";
                    await new BrdInfosApiController().PutBrdInfo(brdInfo.Id, brdInfo);

                    connection.Close();
                    return Json(new { s = "Back to Concept Successfully" });
                }
            }
            catch (Exception ex)
            {
                log.Info("Error in Back to Concept. " + ex.Message.ToString());
                return Json(new { f = "Error in BRD Status Update. " + ex.Message.ToString() });
            }
        }


    }
}
