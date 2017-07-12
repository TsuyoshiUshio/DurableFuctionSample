using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DFSample
{
    public static class ParallelExec
    {
        [FunctionName("ParallelExec")]
        public static async Task<List<string>>  Run(
            [OrchestrationTrigger] DurableOrchestrationContext context, 
            TraceWriter log
        )
        {
            log.Info("ParallelExec started!");
            var tasks = new Task<string>[3];
            tasks[0] = context.CallFunctionAsync<string>(
                "EchoExec",
                "Hello, I'm Tsuyoshi"
                );
            tasks[1] = context.CallFunctionAsync<string>(
                "EchoExec",
                "Hello, I'm Kanio");
            tasks[2] = context.CallFunctionAsync<string>(
                "EchoExec",
                "Hello, I'm NEO");
            await Task.WhenAll(tasks);
            var outputs = new List<string>();
            foreach(Task<string> task in tasks)
            {
                outputs.Add(task.Result);
            }

            log.Info("ParallelExec Done!");
            return outputs;
        }

        [FunctionName("EchoExec")]
        public static string EchoExec (
            [ActivityTrigger] DurableActivityContext context, 
            TraceWriter log)
        {
            
            string message = context.GetInput<string>();
            log.Info($"EchoExec started with '{message}'");
            return message + "\nHi, I'm Azure Fucntions";
        }
    }
}