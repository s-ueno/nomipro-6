using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrchestrationFunctons.Core;
using OrchestrationFunctons.Model;
namespace OrchestrationFunctons.Service
{
    /// <summary>
    /// アカウントエンティティ サービス
    /// </summary>
    public class AccountService : CloudEntityService<Account>
    {
        /// <summary>
        /// プレミアム会員ですか？
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPremiumAccount(Account account)
        {
            return false;
        }
    }
}
