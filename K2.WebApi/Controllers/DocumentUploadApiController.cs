using System;
using System.IO;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using K2.WebApi.Helpers;
using K2.WebApi.Models;
using log4net;
using Microsoft.SharePoint;

namespace K2.WebApi.Controllers
{
    //[Authorize]
    public class DocumentUploadApiController : ApiController
    {
        //String fileToUpload = @"C:\banks\Test_concept.docx";
        String sharePointSite = "http://testportal.local/";
        DocumentHelper helper=new DocumentHelper();
        String documentLibraryName = "PIP_Concepts";
        protected static readonly ILog log = LogManager.GetLogger(typeof (DocumentUploadApiController));
        // GET api/<controller>
        DocumentInfosApiController _documentInfosApi = new DocumentInfosApiController();

        [HttpGet]
        [Route("api/DocumentUploadApi/{user}/{documentTypeId}/{documentName}/{requestId}/{*fileToUpload}")]
        public  string UploadDocument(string user, long documentTypeId, string documentName, string fileToUpload)
        {
            //System.IdentityModel.Claims.
            
                //log.Info("triggered api call");
                try
                {
                   
                    if (!string.IsNullOrEmpty(user))
                    {
                        //}
                        string xy = @"TESTPORTAL\";
                        //Create Tag
                        log.Info("about to upload to sharepoint : " + fileToUpload);
                        helper.PushDocumentToSharepoint(user, documentTypeId, documentName,fileToUpload);
                        log.Info("File Uploaded");
                        //Add DocumentInfo Command
                       
                    }
                    else
                    {
                        log.Info("No user Information found");
                    }
                    return "OK";

                }

                catch (Exception ex)
                {
                    log.Error("Error in UploadDocument " + ex);
                    return "Failed";
                }
            
           
        }


        public void PushDocumentToSharepoint(string user, long documentTypeId,string requestId, string fileToUpload)
        {
            Random generator = new Random();
            String randomNumber = generator.Next(0, 1000000).ToString("D6");
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite spSite = new SPSite(sharePointSite))
                {
                    log.Info("Found site");
                    using (SPWeb spWebWeb = spSite.OpenWeb())
                    {
                        spWebWeb.AllowUnsafeUpdates = true;
                        log.Info("Opened web");
                        //if (!System.IO.File.Exists(fileToUpload))
                        //{
                        //    throw new FileNotFoundException("File not found.", fileToUpload);
                        //}
                        string xy = @"TESTPORTAL\";
                        string folderName = user;
                        log.Info("User is " + folderName);
                        var sourceFolder = spWebWeb.Folders[documentLibraryName];
                        log.Info("sourceFolder " + sourceFolder);
                        try
                        {
                            var destinationFolder = sourceFolder.SubFolders[folderName];
                        }
                        catch (ArgumentException ae)
                        {
                            sourceFolder.SubFolders.Add(folderName);
                            log.Error("Created folder for user " + folderName);
                        }
                        catch (System.IO.FileNotFoundException fe)
                        {
                            log.Error("File not found ");
                            //return "File not found";
                        }

                        log.Info("destination folder " + sourceFolder.SubFolders[folderName]);
                        SPFolder myLibrary = spWebWeb.Folders[documentLibraryName].SubFolders[folderName];

                        log.Info("SP LIST " + myLibrary.Name);
                        // Prepare to upload

                        var des = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        log.Info("server path: " + Path.Combine(des,fileToUpload));
                        log.Info("server path1: " + fileToUpload);
                        String fileName = Path.GetFileName(fileToUpload);
                       
                        string finalFile = string.Empty;
                        log.Info("FileName " + fileName);
                        
                        if (fileName != null)
                        {
                            var finalpath = Path.Combine(des, "PIP_docs", fileName);
                            
                            finalFile = fileName.Replace(" ", "_").Replace(".docx", "_").TrimEnd('_') +
                                               ".docx";
                            FileStream fileStream = File.OpenRead(finalpath);

                            // Upload document
                            myLibrary.Files.Add(finalFile, fileStream, true);
                            //log the uploaded document
                        }

                        log.Info("Final File : " + finalFile);
                        var documentPath = sharePointSite + documentLibraryName + "/" + folderName + "/" + finalFile;
                        log.Info("Final Path : " + documentPath);
                        var documentInfo = new DocumentInfo
                        {
                            DocumentName = fileName,
                            DocumentTypeId = documentTypeId,
                            DocumentPath = documentPath
                            //RequestId = requestId
                            //Tag = tag
                        };
                        // Commit 
                        myLibrary.Update();
                        //Register Document path here
                        _documentInfosApi.PostDocumentInfoSync(documentInfo);
                    }
                }
            });
        }

        public static SPFolder GetSpFolder(SPFolder sourceFolder, string folderName)
        {
            SPFolder destination;

            try
            {
                // return the existing SPFolder destination if already exists
                destination = sourceFolder.SubFolders[folderName];
            }
            catch
            {
                // Create the folder if it can't be found

                destination = sourceFolder.SubFolders.Add(folderName);
            }

            return destination;
        }
    }
}