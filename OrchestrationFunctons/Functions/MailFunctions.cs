using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

using OrchestrationFunctons.Core;
using OrchestrationFunctons.Service;
using OrchestrationFunctons.DTO;
using System.Collections.Generic;
using OrchestrationFunctons.Model;
using System.Linq;

namespace OrchestrationFunctons.Functions
{
    public class MailFunctions
    {
        [FunctionName("SendMail")]
        public static async Task<string> SendMail(
            /* APIキーのないAnonymous（不特定多数）で、get / post リクエストの両方を受け付ける */
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // リクエストの中身
            var stream = req.Body;
            // JSONから復元
            var request = await System.Text.Json.JsonSerializer.DeserializeAsync<Account>(stream);

            var result = await Task.Run(() => $"send {request.AccountID}");

            // 戻り値に設定して終了
            return result;
        }
    }
}
