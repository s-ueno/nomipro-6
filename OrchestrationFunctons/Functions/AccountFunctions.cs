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
            /* API�L�[�̂Ȃ�Anonymous�i�s���葽���j�ŁAget / post ���N�G�X�g�̗������󂯕t���� */
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // �ǂ��ł������A�񓯊��� hello
            var result = await Task.Run(() => "hello");

            // �߂�l�ɐݒ肵�ďI��
            return result;
        }

        /// <summary>
        /// �A�J�E���g�J�݃f��
        /// </summary>
        [FunctionName("RegisterAccount")]
        public static async Task<RegisterAccountResponse> RegisterAccount(
            /* API�L�[�iFunction�j�Apost ���N�G�X�g�̗����̃��N�G�X�g�������󂯕t���� */
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // ���N�G�X�g�̒��g
            var stream = req.Body;
            // JSON���畜��
            var request = await System.Text.Json.JsonSerializer.DeserializeAsync<RegisterAccountRequest>(stream);

            // �N���E�h���Azure Table Storage�Ƃ̂������s���G���e�B�e�B�T�[�r�X
            var svc = await CloudEntityService.GetServiceAsync<AccountService>(log);

            if (await svc.ExistsAsync(request.MailAddress))
            {
                return new RegisterAccountResponse() { IsSuccess = false, Message = "���łɃ��[�U�[�͑��݂��܂�" };
            }

            // �ǉ�
            var newAccount = await svc.AddAsync(request.MailAddress);

            // JSON�ŕԂ�
            return new RegisterAccountResponse() { IsSuccess = true, Message = "�쐬���܂���", Account = newAccount };
        }

        [FunctionName("AllAcount")]
        public static async Task<List<Account>> AllAcount(
            /* API�L�[�iFunction�j�Apost ���N�G�X�g�̗����̃��N�G�X�g�������󂯕t���� */
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // �N���E�h���Azure Table Storage�Ƃ̂������s���G���e�B�e�B�T�[�r�X
            var svc = await CloudEntityService.GetServiceAsync<AccountService>(log);

            var all = await svc.ListAsync();
            return all.ToList();
        }
    }
}
