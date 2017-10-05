using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using K2.WebApi.Controllers;
using K2.WebApi.Models;
using log4net;
using PiPWeb.Services;
using Provisioning.Helpers;
//using SourceCode.Workflow.Client;

namespace PiPWeb.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(DocumentController));
        ProductService service = new ProductService();
        DocumentInfosApiController documentInfosApi=new DocumentInfosApiController();
        //
        // GET: /Document/
        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            //string requestId, string tag, long documentTypeId, string finalpath, string fileName
            string documentName = Request.Form["documentName"];
            long documentTypeId = Convert.ToInt64(Request.Form["documentTypeId"]);
            string documentDescription = Request.Form["documentDescription"];
            try
            {
                //Pass document Id and document name from from through api 
                var tag = (string)Session["tag"];
                var requestId = (string)Session["requestId"];

                log.Info("Tag: " + tag);
                log.Info("requestId " + requestId);
                Random generator = new Random();
                String randomNumber = generator.Next(0, 1000000).ToString("D6");
                string userName = User.Identity.Name.Substring(11);

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        if (fileName != null)
                        {
                            //Temp check for extension to be done in commands
                            var uploadedFileExtension= Path.GetExtension(fileName);
                            string fileEnd = randomNumber + "_" + fileName;
                            string fileNewName = documentName + "_" + randomNumber + uploadedFileExtension;
                            //var path = Path.Combine(Server.MapPath("~/Tmp/Files"), fileEnd);
                            var des = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                            //// var path=Path.Combine(des, "PIP_docs",fileEnd);
                            var finalpath = Path.Combine(des, "PIP_docs", fileNewName);
                            //var path = Path.Combine(Server.MapPath("~/PIP_docs/"), fileNewName);
                            log.Info("Path is " + finalpath);
                            if (tag == "141_concept")
                            {
                                var documentTypeInfos = await service.CachedDocumentTypeInfoList();
                                var documentTypeInfo = documentTypeInfos.SingleOrDefault(x => x.DocumentType == "Concept");
                                var fileExtension = Path.GetExtension(finalpath);
                                log.Info("file extension is " + fileExtension);
                                if (fileExtension != documentTypeInfo.DocumentExtension)
                                {
                                    return Json(new { f = "Your document has the wrong file format" });
                                }
                            }
                            log.Info("fileNewName is " + fileNewName);
                            file.SaveAs(finalpath);

                            await DoUpload(requestId, tag, documentTypeId, documentDescription, finalpath, fileNewName);
                            return Json(new { s = "Your document has been successfully uploaded" });
                        }
                        return Json(new { f = "Your document could not be uploaded" });
                    }
                }
                return Json(new {f = "Your document upload failed"});
            }
            catch (Exception ex)
            {
                log.Error("Error : "+ex);
                return Json(new {f = "Oops!! something went wrong"});
            }
        }

        public async Task<ActionResult> GetDocumentInfoListAsync()
        {
            var requestId = (string)Session["requestId"];
            if (requestId != null)
            {
                var documentInfoList = await service.GetDocumentInfoListByRequestId(requestId);
                return Json(new
                {
                    aaData = documentInfoList
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                aaData = new List<DocumentInfo>()
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            //Get all docInfo with requestId
            var documentInfoList=new List<DocumentInfo>();
             
             if (TempData["documentTypeList"] != null)
             {
                 var documentTypeList = (List<DocumentTypeInfo>) TempData["documentTypeList"];
                 //log.Info("The documentType Info list count is " + documentTypeList.Count);
             }
             //var documentTypeList = await service.CachedDocumentTypeInfoList();
             if (TempData["tag"] != null)
             {

                 //var theTag = (string)TempData["tag"];
                 //var requestId = (string)TempData["requestId"];
                 ////log.Info("Tag is " + theTag);
                 //TempData.Keep("tag");
                 //TempData.Keep("requestId");
                 //TempData["tag"] = theTag;
                 //TempData["requestId"] = requestId;
             }
             else
             {
                 log.Info("Tag is empty");
             }

             //get document Types
             
             try
             {
                 //if (requestId != null)
                 //{
                     
                 //    documentInfoList = await service.GetDocumentInfoListByRequestId(requestId);
                     //TempData.Keep("documentInfoList");
                     //TempData["documentInfoList"] = documentInfoList;
                 //    //Clear the TempData on Save
                 //}
             }
             catch (Exception ex)
             {
                 log.Error("Error Loading Document InfoList");
             }
             return View();
        }

         

         public async Task<bool> DoUpload(string requestId, string tag, long documentTypeId, string documentDescription, string finalpath, string fileName)
        {
            try
            {
                bool isDone = false;
                string xy = @"TESTPORTAL\";
                //user.Substring(10), fileToUpload);
                var documentInfo = new DocumentInfo();

                if (tag == "141_concept")
                {
                    isDone = true;
                    documentInfo = new DocumentInfo
                    {
                        DocumentName = fileName,
                        DocumentTypeId = documentTypeId,
                        DocumentPath = finalpath,
                        Tag = tag,
                        RequestId = requestId,
                        DocumentDescription = documentDescription
                    };

                }

                else if (tag == "BRD")
                {
                    isDone = true;
                    documentInfo = new DocumentInfo
                    {
                        DocumentName = fileName,
                        DocumentTypeId = documentTypeId,
                        DocumentPath = finalpath,
                        Tag = tag,
                        RequestId = requestId,
                        DocumentDescription = documentDescription
                    };
                }
                if (isDone)
                {
                    //Check for Concept Document
                    await documentInfosApi.PostDocumentInfo(documentInfo);
                }
                //Load Document with same Concept Id
                return true;
                //Load Document with same Concept Id
                return true;

            }
            catch (Exception ere)
            {
                log.Error("Error calling service " + ere);
                return false;

            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadDocument(HttpPostedFileBase Doc, string documentName,long documentTypeId)
        {
             service = new ProductService();
            //var fileToUpload = Request.Files["Doc"];
            try
            {
                var theTag = (string)TempData["tag"];
                var requestId = (string)TempData["requestId"];
                
                log.Info("Tag: " + theTag);
                log.Info("requestId " + requestId);
                Random generator = new Random();
                String randomNumber = generator.Next(0, 1000000).ToString("D6");
                string userName = User.Identity.Name;
                //Set RequestId in TempData

                
                if (Doc.ContentLength > 0 && documentTypeId > 0)
                {
                    var fileName = Path.GetFileName(Doc.FileName);

                    if (fileName != null )
                    {

                        string fileEnd = randomNumber + "_" + fileName;
                        string fileNewName = documentName + "_" + randomNumber + ".docx";
                       //var path = Path.Combine(Server.MapPath("~/Tmp/Files"), fileEnd);
                        var des = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                       //// var path=Path.Combine(des, "PIP_docs",fileEnd);
                        var finalpath = Path.Combine(des, "PIP_docs", fileNewName);
                        //log.Info(Path.Combine(des, "PIP_docs", fileNewName));
                        var path = Path.Combine(Server.MapPath("~/Tmp/Files"), fileEnd);

                       // var finalpath = Path.Combine(Server.MapPath("~/Tmp/Files"), fileNewName);
                        Doc.SaveAs(path);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Move(path, finalpath);
                        }
                        //fileToUpload.FileName;
                       
                        try
                        {
                            string xy = @"TESTPORTAL\";
                            //user.Substring(10), fileToUpload);
                            log.Info("About to execute from " + userName);
                            log.Info("User ::: " + userName.Substring(11));
                            if (theTag == "141_concept")
                            {
                               
                                    
                                    
                                    log.Info("Request Id " + requestId);
                                    var conceptInfo = await service.GetConceptInfoByRequestId(requestId);
                                    var documentInfo = new DocumentInfo
                                    {
                                        DocumentName = fileName,
                                        DocumentTypeId = documentTypeId,
                                        DocumentPath = finalpath,
                                        Tag = theTag,
                                        RequestId = requestId
                                    };
                                    documentInfosApi.PostDocumentInfo(documentInfo);
                                

                                //SetDataFielfd(conceptInfo);
                                // service.CreateNewDocument(userName.Substring(11), documentTypeId, documentName, finalpath);
                            }
                            //Load Document with same Concept Id
                            return Json(new { s = "Document created successfully.." });
                           
                        }
                        catch (Exception ere)
                        {
                            log.Error("Error calling service " + ere);
                            return Json(new { f = "Error uploading document" });
                        }
                       
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                log.Error("Error in Uploading document "+ex);
                return Json(new { f = "Document Upload failed" });
            }

        }

    }
}
