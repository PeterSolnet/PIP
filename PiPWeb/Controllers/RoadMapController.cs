using System;
using System.Collections;
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
    public class RoadMapController : Controller
    {
        RoadMapsApiController roadMapMapApiController = new RoadMapsApiController();
        DocumentHelper helper = new DocumentHelper();
        // GET: /RoadMapMaster/
        protected static readonly ILog log = LogManager.GetLogger(typeof(ConceptController));
        ProductService productService = new ProductService();

        public async Task<ActionResult> Index()
        {
            try
            {
                helper.GivePermissionToFolder();
                await GetRoadMapList();
                await GetRoadMapMasterList();

                var aduserList = new List<ADModel>();
                aduserList = await productService.GetLocalADUserList();
                
                TempData.Keep("adUserList");
                TempData["adUserList"] = aduserList;


            }
            catch (Exception ex)
            {

                log.Error("Error " + ex);
            }
            return View();
        }

        public async Task<List<RoadMapViewModel>> GetRoadMapList()
        {
            var roadMapList = await productService.GetRoadMapsList();
            var roadMapMaster = await productService.GetRoadMapMasterList();

            List<RoadMapViewModel> roadMapLists =  (from pd in roadMapList
                                                       join p in roadMapMaster on pd.RoadMapMasterId equals p.Id
                                                       orderby pd.Id
                                                     select new RoadMapViewModel
                                                     {
                                                       Id               = pd.Id,
                                                       RoadMapMasterId  = pd.RoadMapMasterId,
                                                       RoadMapName      = p.RoadMapName,
                                                       ConceptName      = pd.ConceptName,
                                                       ConceptOwner     = pd.ConceptOwner
                                                     }).ToList();

            TempData.Keep("roadMapLists");
            TempData["roadMapLists"] = roadMapLists;
            return roadMapLists;
        }

        public async Task<List<RoadMapMaster>> GetRoadMapMasterList()
        {
            var roadMapMasterList = await productService.GetRoadMapMasterList();
            var newroadMapMasterList = roadMapMasterList.FindAll(x => x.EndDate > DateTime.Today);
            
            TempData.Keep("newroadMapMasterList");
            TempData["newroadMapMasterList"] = newroadMapMasterList;
            return newroadMapMasterList;
        }

        public async Task<ActionResult> CreateRoadMap(RoadMap roadMapInfo)
        {
            try
            {
                await roadMapMapApiController.PostRoadMap(roadMapInfo);
                
                return Json(new { s = "Road Map has been successfully created." });
            }
            catch (Exception ex)
            {
                log.Error("Error occured in create Road Map " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }


        }

        public async Task<ActionResult> UpdateRoadMap(RoadMap roadMapInfo)
        {

            try
            {
                await roadMapMapApiController.PutRoadMap(roadMapInfo.Id, roadMapInfo);
                return Json(new { s = "Road Map has been successfully updated." });

            }
            catch (Exception ex)
            {
                log.Error("Error occured in updating Road Map " + ex);
                return Json(new { f = "Ooops!! something has gone wrong" });
            }
        }
    }
}
