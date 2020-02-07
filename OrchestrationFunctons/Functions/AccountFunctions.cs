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

namespace OrchestrationFunctons
{
    public static class AccountFunctions
    {

        [FunctionName("AnonymousHello")]
        public static async Task<string> AnonymousHello(
            /* APIキーのないAnonymous（不特定多数）で、get / post リクエストの両方を受け付ける */
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // どうでもいい、非同期の hello
            var result = await Task.Run(() => "hello");

            // 戻り値に設定して終了
            return result;
        }

        /// <summary>
        /// アカウント開設デモ
        /// </summary>
        [FunctionName("RegisterAccount")]
        public static async Task<RegisterAccountResponse> RegisterAccount(
            /* APIキー（Function）、post リクエストの両方のリクエストだけを受け付ける */
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // リクエストの中身
            var stream = req.Body;
            // JSONから復元
            var request = await System.Text.Json.JsonSerializer.DeserializeAsync<RegisterAccountRequest>(stream);

            // クラウド上のAzure Table Storageとのやり取りを行うエンティティサービス
            var svc = await CloudEntityService.GetServiceAsync<AccountService>(log);

            if (await svc.ExistsAsync(request.MailAddress))
            {
                return new RegisterAccountResponse() { IsSuccess = false, Message = "すでにユーザーは存在します" };
            }

            // 追加
            var newAccount = await svc.AddAsync(request.MailAddress);

            // JSONで返す
            return new RegisterAccountResponse() { IsSuccess = true, Message = "作成しました", Account = newAccount };
        }

        [FunctionName("AllAcount")]
        public static async Task<List<Account>> AllAcount(
            /* APIキー（Function）、post リクエストの両方のリクエストだけを受け付ける */
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // クラウド上のAzure Table Storageとのやり取りを行うエンティティサービス
            var svc = await CloudEntityService.GetServiceAsync<AccountService>(log);

            var all = await svc.ListAsync();
            return all.ToList();
        }
    }
}
