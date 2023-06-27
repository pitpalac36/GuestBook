using Microsoft.Azure.Cosmos.Table;
using System;

namespace GuestBook.Data
{
    public class Entry: TableEntity
    {
        public Entry()
        {
            PartitionKey = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }

        public string Message { get; set; }
        public string GuestName { get; set; }
        public string PhotoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}