using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using K2.WebApi.Models;
using log4net;
using Newtonsoft.Json;

namespace K2.WebApi.Services
{
    public class ProductApiService
    {
          ILog log = LogManager.GetLogger(typeof(ProductApiService));
        

        public async Task<DocumentInfo> CreateNewDocument(long documentTypeId, string documentName)
        {
             string webApiUrl = ConfigurationManager.AppSettings["webApiUrl"];
            var uri = webApiUrl + "SdpApi/" + documentTypeId + "/" + documentName + "/document";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<DocumentInfo>(
                    httpClient.GetStringAsync(uri).Result
                    );

            }
        }

       
    }
}