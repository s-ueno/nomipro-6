using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using OrchestrationFunctons.Core;
namespace OrchestrationFunctons.Model
{
    /// <summary>
    /// アカウント情報を表す
    /// </summary>
    public class Account : CloudEntity
    {
        /// <summary>
        /// AccuntID という導出プロパティを用意し、実態は<see cref="TableEntity.RowKey"/> を指し示す
        /// </summary>
        [IgnoreProperty]
        public string AccountID { get => RowKey; set => RowKey = value; }

        /// <summary>アカウントの画面表示名</summary>
        public string DisplayName { get; set; }

    }
}
