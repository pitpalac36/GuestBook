using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using GuestBook.Data;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;

namespace GuestBook.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly string picsContainerName = "guestbookpicsblob";
        private readonly string thumbQueueName = "guestbookthumbnails";
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private QueueClient queue;
        private BlobContainerClient container;

        public override void Run()
        {
            Trace.TraceInformation("Listening for queue messages...");

            while (true)
            {
                try
                {
                    QueueMessage msg = queue.ReceiveMessage();
                    if (msg != null)
                    {
                        var imageBlobUri = msg.MessageText;
                        Trace.TraceInformation("Processing image in blob '{0}'.", imageBlobUri);
                        string thumbnailName = System.Text.RegularExpressions.Regex.Replace(imageBlobUri, "([^\\.]+)(\\.[^\\.]+)?$", "$1-thumb$2");

                        BlobClient inputBlob = container.GetBlobClient(imageBlobUri);
                        BlobClient outputBlob = container.GetBlobClient(thumbnailName);
                        if (!outputBlob.Exists())
                        {
                            using (Stream input = inputBlob.OpenRead())
                            using (MemoryStream output = new MemoryStream())
                            {
                                ProcessImage(input, output);
                                output.Position = 0;

                                outputBlob.Upload(output);

                                string thumbnailBlobUri = outputBlob.Uri.ToString();

                                var ds = new DataSource();
                                Entry entry = ds.GetGuestBookEntryByPhotoURL(inputBlob.Uri.ToString());
                                entry.ThumbnailUrl = thumbnailBlobUri;
                                ds.UpdateImageThumbnail(entry);

                                queue.DeleteMessage(msg.MessageId, msg.PopReceipt);

                                Trace.TraceInformation("Generated thumbnail in blob '{0}'.", thumbnailBlobUri);
                            }
                        }
                        else
                        {
                            queue.DeleteMessage(msg.MessageId, msg.PopReceipt);
                        }

                    }
                }
                catch (Azure.RequestFailedException e)
                {
                    Trace.TraceError("Exception when processing queue item. Message: '{0}'", e.Message);
                    Thread.Sleep(5000);
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var storageConnectionString = AppSettings.LoadAppSettings().GetSection("StorageConnectionString").Value;
            container = new BlobContainerClient(storageConnectionString, picsContainerName);
            queue = new QueueClient(storageConnectionString, thumbQueueName);

            Trace.TraceInformation("Creating container and queue...");

            bool storageInitialized = false;
            while (!storageInitialized)
            {
                try
                {
                    container.CreateIfNotExists();
                    container.SetAccessPolicy(PublicAccessType.Blob);
                    queue.CreateIfNotExists();
                    storageInitialized = true;
                }
                catch (Azure.RequestFailedException e)
                {
                    if (e.Status.Equals(HttpStatusCode.NotFound))
                    {
                        Trace.TraceError("Storage services initialization failure. Message: '{0}'", e.Message);
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole started");

            return result;
        }

        public void ProcessImage(Stream input, Stream output)
        {
            int width;
            int height;
            var originalImage = new Bitmap(input);

            width = 128;
            height = 128 * originalImage.Height / originalImage.Width;

            var thumbnailImage = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(thumbnailImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, width, height);
            }

            thumbnailImage.Save(output, ImageFormat.Jpeg);
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole is stopping...");

            cancellationTokenSource.Cancel();
            runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole has stopped");
        }
    }
}
