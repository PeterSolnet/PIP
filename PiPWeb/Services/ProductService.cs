using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using K2.WebApi.Models;
using log4net;
using Newtonsoft.Json;
using Provisioning.Commands;

namespace PiPWeb.Services
{
    public class ProductService
    {
        //string webApiUrl = ConfigurationManager.AppSettings["webApiUrl"];
        protected static readonly ILog log = LogManager.GetLogger(typeof(ProductService));
        string apiUrl = ConfigurationManager.AppSettings["apiUrl"];
        string sdpApiUrl = ConfigurationManager.AppSettings["sdpApiUrl"];
        //"api/DocumentUploadApi/{user}/{documentTypeId}/{documentName}/{fileToUpload}"
        public async Task<string> CreateNewDocument(string user, long documentTypeId, string documentName, string fileToUpload)
        {
            log.Info("The File name " + fileToUpload);
            var uri = apiUrl + "DocumentUploadApi/" + user + "/" + documentTypeId + "/" + documentName   +"/" + fileToUpload;
            log.Info("Loading document to sharepoint" + uri);
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<string>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<StakeHolder> GetStakeHolderInfoById(long id)
        {
            //
            var uri = apiUrl + "/StakeHoldersApi/" + id;

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<StakeHolder>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<ImplementationTimeline>> GetProductImplTimeListByRequestId(string requestId)
        {
            var uri = apiUrl + "/ImplementationTimelinesApi/" + requestId + "/requestId";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ImplementationTimeline>>(
                      await httpClient.GetStringAsync(uri)
                    );
            }
        }
        public async Task<PauseInfo> GetPauseInfoInfoByRequestId(string requestId,string status)
        {
            var uri = apiUrl + "/PauseInfosApi/" + requestId + "/" + status + "/pauseRequest";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<PauseInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<RoadMap> GetRoadMapInfoById(long id)
        {
            var uri = apiUrl + "/RoadMapsapi/" + id;

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<RoadMap>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<RoadMapMaster> GetRoadMapMasterInfoByRoadMapMasterId(long Id)
        {
            var uri = apiUrl + "/RoadMapMastersApi/" + Id + "/RoadMapMasterInfoByRoadMapMasterId";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<RoadMapMaster>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<ADModel>> GetADUserList()
        {
            var uri = apiUrl + "getcachedadusers/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ADModel>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<RoadMapMaster>> GetRoadMapMasterInfoList()
        {
            var uri = apiUrl + "/RoadMapMastersApi/";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<RoadMapMaster>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }

        }
       

        //Get all RoadMaster
        public async Task<List<RoadMap>> GetRoadMapList()
        {
            var uri = apiUrl + "/RoadMapsapi/";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<RoadMap>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<ActionHistoryInfo>> GetActionHistoryInfoListByRequestId(string requestId)
        {
            var uri = apiUrl + "/ActionHistoryInfosApi/" + requestId + "/requestId/";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ActionHistoryInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }

        }


        public async Task<ConceptInfo> GetConceptInfoByRequestId(string requestId)
        {
            var uri = apiUrl + "ConceptInfosApi/" +requestId+"/conceptrequest/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<ConceptInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<ImplementationTimeline>> GetImplementationTimeListByImplementationId(long implementationInfoId)
        {
            var uri = apiUrl + "/ImplementationTimelinesApi/" + implementationInfoId + "/ImplementationInfoId/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ImplementationTimeline>>(
                      await httpClient.GetStringAsync(uri)
                    );
            }
        }
        public async Task<BrdInfo> GetBrdInfoByRequestId(string requestId)
        {
            var uri = apiUrl + "BrdInfosApi/" + requestId + "/brdrequest/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<BrdInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<List<SlaCategory>> GetSlaCategoryList()
        {
            var uri = apiUrl + "SlaCategoriesApi";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<SlaCategory>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<ConceptInfo> GetConceptInfoById(long Id)
        {
            var uri = apiUrl + "ConceptInfosApi/" + Id;
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<ConceptInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }

        public async Task<List<ConceptInfo>> GetConceptInfoList()
        {
            var uri = apiUrl + "/ConceptInfosApi/";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ConceptInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }

        }
        public async Task<List<DocumentTypeInfo>> CachedDocumentTypeInfoList()
        {
            var uri = apiUrl + "/CachedDocumentTypeInfos/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<DocumentTypeInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        //[ResponseType(typeof(List<DocumentInfo>))]
        public async Task<List<DocumentInfo>> GetDocumentInfoListByRequestId(string requestId)
        {
            var uri = apiUrl + "/DocumentInfosApi/"+requestId+"/request";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<DocumentInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
        public async Task<HttpResponseMessage> GetDocumentInfoListResponseByRequestId(string requestId)
        {
            var uri = apiUrl + "/DocumentInfosApi/" + requestId + "/request";
            using (HttpClient httpClient = new HttpClient())
            {
                return  httpClient.GetAsync(uri).Result;

            }
        }
        //HttpResponseMessage getResponse = client.GetAsync("DocumentInfosApi/" + requestId + "/request").Result;
        public async Task<CommandStatus> CreateConcept(string userId, string message, string conceptName, string description, bool isNewConcept, string conceptOwner, string requestId)
        {
            log.Info("User: " + userId + "message: " + message + " conceptname: " + conceptName + " description: " + description + " isNewConcept: " + isNewConcept + " conceptOwner: " + conceptOwner + " requestId: " + requestId);
            //api/SdpApi/{userId}/{message}/{conceptName}/{description}/{isNewConcept}
            var uri = sdpApiUrl + "SdpApi/" + userId + "/" + message + "/" + conceptName + "/" + description + "/" + isNewConcept + "/" + conceptOwner + "/" + requestId;
            log.Info("uri is : "+uri);
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<CommandStatus>(
                       httpClient.GetStringAsync(uri).Result
                    );

            }

        }
        public async Task<List<DocumentTypeInfo>> GetDocumentTypeList()
        {
            var uri = apiUrl + "/DocumentTypeInfosApi/";

            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<DocumentTypeInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
           
        }

        public async Task<List<TaskInfo>> GetTaskInfoListByRequestId(string requestId)
        {
            var uri = apiUrl + "/TaskInfosApi/" + requestId + "/request";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<TaskInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }

        public async Task<PauseInfo> GetPauseInfoByRequestIdAndStatus(string requestId, string status)
        {
            var uri = apiUrl + "PauseInfosApi/" + requestId + "/" + status + "/pauseRequest";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<PauseInfo>(
                      await httpClient.GetStringAsync(uri)
                    );
            }
        }

        public async Task<PauseInfo> GetPauseInfoById(long Id)
        {
            var uri = apiUrl + "pauseinfosapi/" + Id;
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<PauseInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }

        public async Task<List<ActivityInfo>> GetActivityInfoList()
        {
            var uri = apiUrl + "ActivityInfosApi/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ActivityInfo>>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }

        public async Task<ImplementationInfo> GetImplementationInfoByRequestIdandTag(string requestId, string tag)
        {
            var uri = apiUrl + "ImplementationInfosApi/" + requestId + "/"+ tag +"/GetImplementationByRequestIDandTag";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<ImplementationInfo>(
                      await httpClient.GetStringAsync(uri)
                    );

            }
        }
       
       
    }
}