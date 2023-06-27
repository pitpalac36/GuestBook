using System;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using GuestBook.Data;

namespace GuestBook.WebRole
{
    public partial class _Default : Page
    {
        private readonly string containerName = "guestbookpicsblob";
        private static bool _isStorageInitialized = false;
        private static object _lock = new object();
        private static BlobContainerClient _blobContainerClient;
        private static QueueClient _queueClient;
        private static DataSource ds = new DataSource();

        public void Page_Init(object o, EventArgs e)
        {
            ObjectDataSource1.TypeName = typeof(DataSource).AssemblyQualifiedName;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Timer1.Enabled = true;
            }
        }

        public void SignButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                InitializeStorage();

                string uniqueBlobName = string.Format("image_{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(FileUpload1.FileName));

                BlobClient blob = _blobContainerClient.GetBlobClient(uniqueBlobName);

                using (var fileStream = FileUpload1.PostedFile.InputStream)
                {
                    blob.Upload(fileStream);
                }
                System.Diagnostics.Trace.TraceInformation("Upload image '{0}' to blob storage as '{1}'", FileUpload1.FileName, uniqueBlobName);

                var entry = new Entry()
                {
                    GuestName = NameTextBox.Text,
                    Message = MessageTextBox.Text,
                    PhotoUrl = blob.Uri.ToString(),
                    ThumbnailUrl = blob.Uri.ToString()
                };
                ds.AddEntry(entry);
                System.Diagnostics.Trace.TraceInformation("Added entry {0}-{1} in table storage for guest '{2}'", entry.PartitionKey, entry.RowKey, entry.GuestName);

                if (_queueClient.Exists())
                {
                    var message = uniqueBlobName;
                    _queueClient.SendMessage(message);
                    System.Diagnostics.Trace.TraceInformation("Sent message to process blob '{0}'", uniqueBlobName);
                }
            }

            NameTextBox.Text = string.Empty;
            MessageTextBox.Text = string.Empty;

            DataList1.DataBind();
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            DataList1.DataBind();
        }

        private void InitializeStorage()
        {
            if (_isStorageInitialized)
            {
                return;
            }

            lock (_lock)
            {
                if (_isStorageInitialized)
                {
                    return;
                }

                try
                {
                    var storageConnectionString = AppSettings.LoadAppSettings().GetSection("StorageConnectionString").Value;

                    _blobContainerClient = new BlobContainerClient(storageConnectionString, containerName);
                    _blobContainerClient.CreateIfNotExists();

                    _blobContainerClient.SetAccessPolicy(PublicAccessType.Blob);

                    string queueName = "guestbookthumbnails";
                    _queueClient = new QueueClient(storageConnectionString, queueName);
                    _queueClient.CreateIfNotExists();
                }
                catch (WebException)
                {
                    throw new WebException("Storage services initialization failure." +
                        "Check your storage acceount configuration settings." +
                        "If running locally, ensure that the Development Storage service is running.");
                }

                _isStorageInitialized = true;
            }
        }

        protected void Image_Click1(object sender, ImageClickEventArgs e)
        {
            /*System.Diagnostics.Trace.TraceInformation("Clicked");
            ImageButton imageBtn = sender as ImageButton;
            ImageFull.ImageUrl = imageBtn.Attributes["FullImageUrl"].ToString();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "showImageModal", "$('#imageModal').modal('show');", true);
            upModal.Update();
            */

            ImageButton imageBtn = sender as ImageButton;
            string imageUrl = imageBtn.Attributes["FullImageUrl"].ToString();
            Response.Redirect("~/ImagePage.aspx?imageUrl=" + Server.UrlEncode(imageUrl));
        }
    }
}