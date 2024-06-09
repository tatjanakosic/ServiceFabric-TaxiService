using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public class AzureStorageHelper
    {
        private CloudTable _table;

        public AzureStorageHelper(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
            _table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public void InsertEntity<T>(T entity) where T : ITableEntity
        {
            TableOperation insertOperation = TableOperation.Insert(entity);
            _table.ExecuteAsync(insertOperation).GetAwaiter().GetResult();
        }

        public void InsertOrMergeEntity<T>(T entity) where T : ITableEntity
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            _table.ExecuteAsync(insertOrMergeOperation).GetAwaiter().GetResult();
        }

        public async Task InsertOrMergeEntityAsync<T>(T entity) where T : ITableEntity
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await _table.ExecuteAsync(insertOrMergeOperation);
        }


        public T RetrieveEntity<T>(string partitionKey, string rowKey) where T : ITableEntity, new()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult retrievedResult = _table.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();
            return (T)retrievedResult.Result;
        }

        public void DeleteEntity<T>(T entity) where T : ITableEntity
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            _table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(TableQuery<T> query) where T : ITableEntity, new()
        {
            var result = new List<T>();
            TableContinuationToken token = null;

            do
            {
                var queryResult = await _table.ExecuteQuerySegmentedAsync(query, token);
                result.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return result;
        }
    }
}
