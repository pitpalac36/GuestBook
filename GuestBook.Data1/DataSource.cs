using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;

namespace GuestBook.Data
{
    public class DataSource
    {
        private static readonly string _tableName = "Entry";

        private static CloudStorageAccount _storageAccount;
        private static CloudTable _table;


        static DataSource()
        {
            var storageConnectionString = AppSettings.LoadAppSettings().GetSection("StorageConnectionString").Value;

            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _table = tableClient.GetTableReference(_tableName);
            _table.CreateIfNotExists();
        }

        public IEnumerable<Entry> GetEntries()
        {
            var entities = _table.ExecuteQuery(new TableQuery<Entry>()).OrderBy(e => e.RowKey).ToList();
            return entities;  
        }

        public Entry GetGuestBookEntryByPhotoURL(string photoUrl)
        {

            TableQuery<Entry> query =
                new TableQuery<Entry>().Where(
                    TableQuery.GenerateFilterCondition("PhotoUrl", QueryComparisons.Equal, photoUrl));
            var entities = _table.ExecuteQuery(query);
            var entity = entities.ElementAt(0);
            return entity;
        }

        public void EditGuestBookEntryThumbUrl(string thumbnailUrl, string photoUrl)
        {
            TableQuery<Entry> query =
                new TableQuery<Entry>().Where(
                    TableQuery.GenerateFilterCondition("PhotoUrl", QueryComparisons.Equal, photoUrl));
            var entities = _table.ExecuteQuery(query);
            var entity = entities.ElementAt(0);
            entity.ThumbnailUrl = thumbnailUrl;
            TableOperation updateOperation = TableOperation.Merge(entity);
            _table.Execute(updateOperation);
        }

        public void AddEntry(Entry newItem)
        {
            TableOperation tableOperation = TableOperation.Insert(newItem);
            _table.Execute(tableOperation);
        }

        public void UpdateImageThumbnail(Entry entry)
        {
            TableOperation updateOperation = TableOperation.Merge(entry);
            _table.Execute(updateOperation);
        }
    }
}
