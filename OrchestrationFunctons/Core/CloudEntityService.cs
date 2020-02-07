
// https://tech.guitarrapc.com/entry/2019/01/24/041510#WindowsAzureStorage-%E3%81%A8-MicrosoftAzureStorage-%E3%81%AE%E9%81%95%E3%81%84
// using Microsoft.WindowsAzure.Storage;
// using Microsoft.WindowsAzure.Storage.Table ;
//              ↑ vs ↓
// using Microsoft.Azure.Cosmos.Table;

// Microsoft.Azure.Cosmos.Table が最も期待されているライブラリだが、
// FaaS は Microsoft.NET.Sdk.Functions　というライブラリで構成されており
// 内部的には Microsoft.Azure.WebJobs.Host.Storage の中で
// WindowsAzure.Storage に依存している。
// <see cref="TableEntity"/> クラスは Microsoft.Azure.Cosmos.Table  と
// WindowsAzure.Storage に両方存在していて、両立はできない。

// そのため、TableEntityを使う場合には 現時点では WindowsAzure.Storage 一択


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OrchestrationFunctons.Core
{
    public interface ILoggerHolder
    {
        ILogger Logger { get; set; }
    }

    public interface ICloudEntityService
    {
        Task InitializeAsync();
    }

    public abstract class CloudEntityService
    {
        public static async Task<TService> GetServiceAsync<TService>(ILogger log = null)
            where TService : ICloudEntityService, new()
        {
            var result = new TService();
            if (result is ILoggerHolder holder)
            {
                holder.Logger = log;
            }
            await result.InitializeAsync();
            return result;
        }
    }

    public class CloudEntityService<T> : CloudEntityService, ICloudEntityService
        where T : TableEntity, new()
    {
        private CloudTableClient _tableClient;
        private CloudTable _table;

        protected CloudTable Table { get { return _table; } }

        protected CloudEntityService() : this(null)
        {
        }

        protected CloudEntityService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = AppUtil.GetAppConfigValue("AzureWebJobsStorage", "");
            }
            var strageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = strageAccount.CreateCloudTableClient();
        }

        async Task ICloudEntityService.InitializeAsync()
        {
            await InitializeAsync(typeof(T).Name);
        }

        protected async Task InitializeAsync(string tableName)
        {
            _table = _tableClient.GetTableReference(tableName);
            await _table.CreateIfNotExistsAsync();
        }

        public static async Task<CloudEntityService<T>> GetServiceAsync()
        {
            var connectionString = AppUtil.GetAppConfigValue("AzureWebJobsStorage", "");
            var tableName = typeof(T).Name;
            return await GetServiceAsync(connectionString, tableName);
        }
        public static async Task<CloudEntityService<T>> GetServiceAsync(string connectionString)
        {
            var tableName = typeof(T).Name;
            return await GetServiceAsync(connectionString, tableName);
        }
        public static async Task<CloudEntityService<T>> GetServiceAsync(string connectionString, string tableName)
        {
            var result = new CloudEntityService<T>(connectionString);
            await result.InitializeAsync(tableName);
            return result;
        }
        public async Task<T> FindAsync(string rowKey)
        {
            return await FindAsync(typeof(T).Name, rowKey);
        }
        public async Task<T> FindAsync(string partitionKey, string rowKey)
        {
            var ope = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var retrieveResult = await _table.ExecuteAsync(ope);
            if (retrieveResult.Result == null) { return null; }
            return (T)(retrieveResult.Result);
        }
        public async Task<bool> ExistsAsync(string rowKey)
        {
            return await ExistsAsync(typeof(T).Name, rowKey);
        }
        public async Task<bool> ExistsAsync(string partitionKey, string rowKey)
        {
            var ret = await FindAsync(partitionKey, rowKey);
            return ret != null;
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
            var query = new TableQuery<T>();
            var retrieveResult = await _table.ExecuteQuerySegmentedAsync(query, null);
            return retrieveResult.Results;
        }

        public async Task<IEnumerable<T>> FindByQueryAsync(TableQuery<T> query)
        {
            var retrieveResult = await _table.ExecuteQuerySegmentedAsync(query, null);
            return retrieveResult.Results;
        }

        public async Task<T> AddAsync(string rowKey)
        {
            return await AddAsync(typeof(T).Name, rowKey);
        }
        public async Task<T> AddAsync(string partitionKey, string rowKey)
        {
            if ((await FindAsync(partitionKey, rowKey)) != null) { return null; }

            var newItem = new T() { PartitionKey = partitionKey, RowKey = rowKey };
            var ret = await _table.ExecuteAsync(TableOperation.Insert(newItem));
            return (T)ret.Result;
        }

        public async Task UpdateAsync(T item)
        {
            await _table.ExecuteAsync(TableOperation.InsertOrReplace(item));
        }

        public async Task UpdateLockAsync(T item)
        {
            try
            {
                TableOperation operation = TableOperation.Replace(item);
                await this.Table.ExecuteAsync(operation);
            }
            catch (StorageException ex)
            {
                throw ex;
            }
        }
        public async Task DeleteAsync(string partitionKey, string rowKey)
        {
            var item = await FindAsync(partitionKey, rowKey);
            if (item == null) { return; }

            var ope = TableOperation.Delete(item);
            await _table.ExecuteAsync(ope);
        }
    }

}
