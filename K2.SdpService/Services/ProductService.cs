using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using K2.WebApi.Models;
using Newtonsoft.Json;

namespace K2.SdpService.Services
{
    public class ProductService
    {
        string webApiUrl = ConfigurationManager.AppSettings["webApiUrl"];

        public async Task<DocumentInfo> CreateNewDocument(long documentTypeId,string documentName)
        {
            var uri = webApiUrl + "SdpApi/" + documentTypeId + "/" + documentName + "/document";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<DocumentInfo>(
                    await httpClient.GetStringAsync(uri)
                    );
            }
        }
    }
}