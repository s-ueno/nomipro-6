using Microsoft.WindowsAzure.Storage.Table;
using OrchestrationFunctons.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrchestrationFunctons.Model
{
    /// <summary>
    /// 採番サービス用のエンティティ
    /// </summary>
    public class IDNumbering : CloudEntity
    {
        [IgnoreProperty]
        public string Name { get => RowKey; set => RowKey = value; }
        public int Number { get; set; }
    }
}
