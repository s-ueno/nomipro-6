using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrchestrationFunctons.Core
{
    public abstract class CloudEntity : TableEntity
    {
        [IgnoreProperty]
        public string DefaultPartitionKey { get; set; }

        protected CloudEntity()
        {
            PartitionKey =
            DefaultPartitionKey = this.GetType().Name;
        }
    }
}
