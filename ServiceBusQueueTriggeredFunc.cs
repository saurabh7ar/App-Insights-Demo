using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class ServiceBusQueueTriggeredFunc
    {
        [FunctionName("ServiceBusQueueTriggeredFunc")]
        public async Task RunAsync([ServiceBusTrigger("%QueueName%", Connection = "ConString")]string myQueueItem, ILogger log)
        {
            var guid = Guid.NewGuid();
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            log.LogInformation(string.Format("RequestID:{0} - Sending Details to find providers in {1}", guid,myQueueItem));
            var response = await GetNpiData("2.1", myQueueItem);
            log.LogInformation(string.Format("RequestID:{0} - Received provider details for {1} - {2}", guid,myQueueItem, response));


        }

        public static async Task<string> GetNpiData(string version, string city)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("https://npiregistry.cms.hhs.gov");
                
                var response = await client.GetAsync($"/api?version={version}&city={city}");
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
