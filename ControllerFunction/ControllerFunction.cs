using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace DFSample
{
    public static class ControllerFunction
    {
        [FunctionName("ControllerFunction")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "controller/{functionName}") ] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            string functionName,
            TraceWriter log)
        {
            log.Info($"ControllerFunction controller/{functionName} was triggered!");

            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync(functionName, eventData);
            log.Info($"Started controller with ID = '{instanceId}'");

            return starter.CreateCheckStatusResponse(req, instanceId);


        }
    }
}