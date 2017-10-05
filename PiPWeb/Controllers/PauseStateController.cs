using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using K2.WebApi.Controllers;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using Newtonsoft.Json;
using PiPWeb.Services;
using SourceCode.Workflow.Client;

namespace PiPWeb.Controllers
{
    public class PauseStateController : Controller
    {
        // GET: /Product/
        protected static readonly ILog log = LogManager.GetLogger(typeof(PauseStateController));
        ConceptInfosApiController c = new ConceptInfosApiController();
        PauseInfosApiController pauseInfoApi = new PauseInfosApiController();
        DocumentHelper helper = new DocumentHelper();
        ProductService productService = new ProductService();

        [Authorize]
        public async Task<ActionResult> Index(string SN)
        {

            await LoadAllAdUser();

            string impersonatedUser = @"K2:" + User.Identity.Name;
            var brdInfo = new BrdInfo();
            using (var connection = new Connection())
            {
                var pauseInfo = new PauseInfo();
                try
                {
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);

                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);

                    string requestId = (string)worklistItem.ProcessInstance.DataFields["RequestID"].Value;

                    TempData.Keep("SN");
                    TempData["SN"] = SN;
                     pauseInfo = await productService.GetPauseInfoByRequestIdAndStatus(requestId, "Paused");
                    //var pauseInfoReturned =  pauseInfo.FirstOrDefault(x => x.Status != "Resumed");

                    //TempData.Keep("pauseInfo");
                    //TempData["pauseInfo"] = pauseInfo;
                }
                catch (Exception ex)
                {

                    log.Error("Error Loading WorkListItem" + ex);
                    // return RedirectToAction("PipWorkList");
                }

                connection.Close();
                return View(pauseInfo);
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

        public async Task<ActionResult> Update(long pauseRef, string SN)
        {
            //
            string impersonatedUser = @"K2:" + User.Identity.Name;
            using (var connection = new Connection())
            {
                try
                {
                    log.Info(SN);
                    connection.Open("localhost");
                    connection.ImpersonateUser(impersonatedUser);
                    WorklistItem worklistItem = connection.OpenWorklistItem(SN);
                    worklistItem.ProcessInstance.DataFields["DestinationUser"].Value = impersonatedUser;

                    worklistItem.Actions["Unpause"].Execute();

                    PauseInfo pausedInfo = await productService.GetPauseInfoById(pauseRef);
                    pausedInfo.Status = "Resumed";
                    pausedInfo.DateResumed = DateTime.Now;
                    await pauseInfoApi.PutPauseInfo(pauseRef, pausedInfo);
                    connection.Close();
                    return Json(new {s = "Task has been successfully updated."});

                }
                catch (Exception ex)
                {
                    log.Error("Error occured while trying to update the specified task" + ex);
                    connection.Close();
                    return Json(new {f = "Ooops!! something has gone wrong"});
                }
            }
            
        }

    }
}
