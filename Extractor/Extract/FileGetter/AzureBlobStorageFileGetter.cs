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

        /// <summary>
        /// Get files Creation timeStamp, size, and path info of the specified destination.
        /// </summary>
        /// <param name="destination">Target site or folder. No use here.</param>
        /// <param name="searchOption">Determin search files whether loop into subdirectories.</param>
        /// <param name="fileExtention">The file extention which need to transform.</param>
        /// <param name="timeZoneOffset">zone offset base one UTC.</param>
        /// <returns>List of files with Creation timeStamp, size, and path info.</returns>
        public List<Tuple<DateTime, long, string>> GetFilesDetailInfo(string destination, System.IO.SearchOption searchOption, int timeZoneOffset, string fileExtention = null)
        {
            var fileInfoList = new List<Tuple<DateTime, long, string>>();
            var container = GetContainer(Container);
            var blobs = container.ListBlobs(useFlatBlobListing: true);
            foreach (IListBlobItem item in blobs)
            {
                var blob = (CloudBlockBlob)item;
                if (string.IsNullOrEmpty(fileExtention) || blob.Name.EndsWith(fileExtention))
                {
                    fileInfoList.Add(new Tuple<DateTime, long, string>(blob.Properties.LastModified.Value.DateTime.AddHours(timeZoneOffset), blob.Properties.Length, blob.Name));
                }
            }

            return fileInfoList;
        }

        public System.IO.Stream DownLoadFile(string filePath)
        {
            var container = GetContainer(Container);
            var blockBlob = container.GetBlockBlobReference(filePath);

            return blockBlob.OpenRead();
        }

        private CloudBlobContainer GetContainer(string containerName) {
            var storageCredentials = new StorageCredentials(StorageName, StorageKey);
            var account = new CloudStorageAccount(storageCredentials, true);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            return container;
        }
    }
}
