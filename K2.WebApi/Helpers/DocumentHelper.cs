using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using K2.WebApi.Controllers;
using K2.WebApi.Models;
using log4net;
using Microsoft.SharePoint;

namespace K2.WebApi.Helpers
{
    public class DocumentHelper
    {
        String sharePointSite = "http://testportal.local/";

        String documentLibraryName = "PIP_Concepts";
        DocumentInfosApiController _documentInfosApi = new DocumentInfosApiController();
        protected static readonly ILog log = LogManager.GetLogger(typeof(DocumentHelper));
        public void GivePermissionToFolder()
        {
            try
            {
                var des = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var finalpath = Path.Combine(des, "PIP_docs");
                if (!Directory.Exists(finalpath))
                {
                    Directory.CreateDirectory(finalpath);
                    DirectoryInfo directoryInfo = new DirectoryInfo(finalpath);
                    DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                    directorySecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    directoryInfo.SetAccessControl(directorySecurity);
                    log.Info(directoryInfo.GetAccessControl());
                }
            }
            catch (Exception ex)
            {

                log.Error("Error giving permission to folder " + ex);
            }
        }
        public void PushDocumentToSharepoint(string user, long documentTypeId,string documentName ,string fileToUpload)
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
                        HttpServerUtilityBase serverUtility = null;
                        //var theAddress = serverUtility.MapPath("~/Tmp/Files");
                        //log.Info("server path: " + theAddress);
                        String fileName = Path.GetFileName(fileToUpload);
                        string finalFile = string.Empty;
                        log.Info("FileName " + fileName);
                        if (fileName != null)
                        {
                            finalFile = fileName.Replace(" ", "_").Replace(".docx", "_").TrimEnd('_') +
                                               ".docx";
                            log.Info("Final file now is " + finalFile);
                            //FileStream fileStream = File.OpenRead(fileToUpload);

                            // Upload document
                            //myLibrary.Files.Add(finalFile, fileStream, true);
                            //log the uploaded document
                        }

                        log.Info("Final File : " + finalFile);
                        var documentPath = sharePointSite + documentLibraryName + "/" + folderName + "/" + finalFile;
                        log.Info("Final Path : " + documentPath);
                        
                        
                        // Commit 
                        myLibrary.Update();
                        //Register Document path here
                        //_documentInfosApi.PostDocumentInfoSync(documentInfo);
                    }
                }
            });
        }
    }
}