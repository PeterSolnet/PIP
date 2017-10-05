using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using K2.WebApi.Models;
using Newtonsoft.Json;

namespace Provisioning.Services
{
    public class ProvisionService
    {
        string webApiUrl = ConfigurationManager.AppSettings["webApiUrl"];

        public async Task<List<ADModel>> GetADUserInfoList()
        {
            var uri = webApiUrl + "getcachedadusers/";
            using (HttpClient httpClient = new HttpClient())
            {
                return JsonConvert.DeserializeObject<List<ADModel>>(
                     await httpClient.GetStringAsync(uri)
                    );
            }
        }
    }
}