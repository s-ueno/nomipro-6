using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace OrchestrationFunctons.FunctionsMesh
{
    public static class DirectMail
    {
        [FunctionName("DirectMailAll")]
        public static async Task<HttpResponseMessage> SendAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DirectMailMesh", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("DirectMailMesh")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            // AllAcount のFunction経由で全アカウント取得
            var list = await context.CallActivityAsync<List<Model.Account>>("AllAcount", null);

            var tasks = new List<Task<string>>();
            foreach (var each in list)
            {
                // AllAcount のFunction経由で全アカウント取得
                tasks.Add(context.CallActivityAsync<string>("SendMail", null));
            }

            // 
            var result = await Task.WhenAll(tasks);
            // 
            return result.ToList();
        }




    }
}