using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Polly;

namespace PrimePenguin.Libs.LogisticsProviderFileBackup
{

    public enum ContentType
    {
        Json,
        Xml
    }
    
    public enum FileType
    {
        Order,
        OrderConfirmation,
        PurchaseOrder,
        PurchaseOrderConfirmation
    }
    
    public interface IBlobStorageFileBackup
    {
        Task Upload(int tenantId, string filename, string content, ContentType contentType, FileType fileType);
    }
    
    public class BlobStorageFileBackup : IBlobStorageFileBackup
    {
        
        private readonly CloudBlobClient _client;
        private readonly string _container;
        private readonly string _baseFolder;

        public BlobStorageFileBackup(string container, string baseFolder, string storageConnectionString)
        {
            _baseFolder = baseFolder.EndsWith("/") ? baseFolder : baseFolder + "/";
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _client = storageAccount.CreateCloudBlobClient();
            _container = container;
        }
        
        private CloudBlockBlob GetBlobReference(string filename, ContentType type)
        {
            var container = _client.GetContainerReference(_container);
            var blobRef = container.GetBlockBlobReference(filename);
            blobRef.Properties.ContentType = GetContentTypeString(type);
            return blobRef;
        }

        private static string GetContentTypeString(ContentType type)
        {
            return type switch
            {
                ContentType.Json => "application/json",
                ContentType.Xml => "application/xml",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private string GetFullName(int tenantId, string filename, FileType type)
        {
            return $"{_baseFolder}{tenantId}/{GetTypeFolder(type)}/{filename}";
        }
        
        private static string GetTypeFolder(FileType type)
        {
            return type switch
            {
                FileType.Order => "orders",
                FileType.OrderConfirmation => "confirmations",
                FileType.PurchaseOrder => "purchaseOrders",
                FileType.PurchaseOrderConfirmation => "purchaseOrderConfirmations",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        private static async Task DoUpload(CloudBlockBlob blob, string content)
        {
            await Policy.Handle<Exception>()
                .RetryAsync(3)
                .ExecuteAsync(async () => await blob.UploadTextAsync(content));
        }

        public async Task Upload(int tenantId, string filename, string content, ContentType contentType, FileType fileType)
        {
            var name = GetFullName(tenantId, filename, fileType);
            var blob = GetBlobReference(name, contentType);
            await DoUpload(blob, content);
        }
    }
}