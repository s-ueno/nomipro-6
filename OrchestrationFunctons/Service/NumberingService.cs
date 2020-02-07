
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using OrchestrationFunctons.Core;
using OrchestrationFunctons.Model;

namespace OrchestrationFunctons.Service
{
    public class NumberingService : CloudEntityService<IDNumbering>
    {
        public async Task<int> GenerateIDAsync(string name)
        {
            return await GenerateIDAsync(nameof(IDNumbering), name);
        }
        public async Task<int> GenerateIDAsync(string partitionKey, string name)
        {
            return await GenerateIDAsync(partitionKey, name, 100);
        }

        protected async Task<int> GenerateIDAsync(string partitionKey, string rowKey, int threshold)
        {
            var row = await this.FindAsync(partitionKey, rowKey);
            if (row == null)
            {
                await this.AddAsync(partitionKey, rowKey);
                return await GenerateIDAsync(partitionKey, rowKey);
            }
            try
            {
                var number = ++row.Number;
                TableOperation replace = TableOperation.Replace(row);

                await this.Table.ExecuteAsync(replace);

                return number;
            }
            catch (StorageException ex)
            {
                if (threshold == 0)
                {
                    throw;
                }

                if (ex.RequestInformation.HttpStatusCode == 412)
                {
                    await Task.Delay(10);
                    return await GenerateIDAsync(partitionKey, rowKey, --threshold);
                }
                else
                {
                    throw;
                }

            }
        }
    }
}
