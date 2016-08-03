using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Extractor.Extract
{
    public class AzureBlobStorageFileGetter : IFileGetter
    {
        string StorageName, StorageKey, Container;
        public AzureBlobStorageFileGetter(string storageName, string storageKey, string container)
        {
            this.StorageName = storageName;
            this.StorageKey = storageKey;
            this.Container = container;
        }

        public List<Tuple<DateTime, long, string>> GetFilesDetailInfo(string destination, System.IO.SearchOption searchOption, string fileExtention = null)
        {
            List<Tuple<DateTime, long, string>> fileInfoList = new List<Tuple<DateTime, long, string>>();

            StorageCredentials storageCredentials = new StorageCredentials(StorageName, StorageKey);
            CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(Container);
            var blobs = container.ListBlobs(useFlatBlobListing: true);

            foreach (IListBlobItem item in blobs)
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                if(blob.Name.EndsWith(fileExtention))
                    fileInfoList.Add(new Tuple<DateTime, long, string>(blob.Properties.LastModified.Value.DateTime, blob.Properties.Length, blob.Name));
            }
            return fileInfoList;
        }

        public System.IO.Stream DownLoadFile(string filePath)
        {
            StorageCredentials storageCredentials = new StorageCredentials(StorageName, StorageKey);
            CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(Container);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);

            MemoryStream memStream = new MemoryStream();
            blockBlob.DownloadToStream(memStream);
            return memStream;

        }
    }
}
