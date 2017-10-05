using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using K2.WebApi.Controllers;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using PiPWeb.Services;

namespace PiPWeb.Controllers
{
    public class RoadMapMasterController : Controller
    {
        RoadMapMastersApiController roadMapMastersApiController = new RoadMapMastersApiController();
        DocumentHelper helper = new DocumentHelper();
        // GET: /RoadMapMaster/
        protected static readonly ILog log = LogManager.GetLogger(typeof(RoadMapMastersApiController));
        ProductService productService = new ProductService();

        public async Task<ActionResult> Index()
        {
            try
            {
                helper.GivePermissionToFolder();
                await GetRoadMapMasters();
            }
            catch (Exception ex)
            {

                log.Error("Error " + ex);
            }
            return View();
        }

        public async Task<List<RoadMapMaster>> GetRoadMapMasters()
        {
            var roadMapMasterList = await productService.GetRoadMapMasterList();
            var newroadMapMasterList = roadMapMasterList.ToList();

                TempData.Keep("newroadMapMasterList");
                TempData["newroadMapMasterList"] = newroadMapMasterList;
                log.Info(newroadMapMasterList);
                return newroadMapMasterList;
        }

        public async Task<ActionResult> CreateRoadMapMaster(RoadMapMaster roadMapMasterInfo)
        {
            
            try
            {
                await roadMapMastersApiController.PostRoadMapMaster(roadMapMasterInfo);
                return Json(new { s = "Road Map Master has been successfully created." });
            }
            catch (Exception ex)
            {
                log.Error("Error occured in create Road Map Master " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }


        }

        public async Task<ActionResult> UpdateRoadMapMaster(RoadMapMaster roadMapMasterInfo)
        {
            
            try
            {
                await roadMapMastersApiController.PutRoadMapMaster(roadMapMasterInfo.Id, roadMapMasterInfo);
                return Json(new { s = "Road Map Master has been successfully updated." });
                
            }
            catch (Exception ex)
            {
                log.Error("Error occured in updating Road Map Master " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }
        }
    }
}
