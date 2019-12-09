using FormBot.Common.Constants;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace FormBot.Common.Helpers
{
    public class FileStorageHelper : ConfigurationBase
    {
        private string blobConnectionString;
        private string blobContainerName;
        private string blobConfigurationFileName;

        public FileStorageHelper()
        {
            // Get the connectionStrings from AppSettings
            this.blobConnectionString = Configuration.GetSection(ConnectionStringConstants.blobStorageConnectionString)?.Value;
            this.blobContainerName = Configuration.GetSection(ConnectionStringConstants.blobContainerName)?.Value;
            this.blobConfigurationFileName = Configuration.GetSection(ConnectionStringConstants.configFileName)?.Value;
        }

        public string GetFileAsStringFromBlob()
        {
            // Gets the file as string
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.blobConnectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(this.blobContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(this.blobConfigurationFileName);
            string contents = blockBlob.DownloadText();
            return contents;
        }

        public T GetFileAsJsonTypeFromBlob<T>()
        {
            // Generate JsonObject of type T of the file configured in appSettings
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.blobConnectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(this.blobContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(this.blobConfigurationFileName);
            string contents = blockBlob.DownloadText();
            return JsonConvert.DeserializeObject<T>(contents);
        }

        public T GetFileAsJsonTypeFromBlob<T>(string fileName)
        {
            // Generate JsonObject of type T of the file configured in based on file name in Storage configured in AppSettings
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.blobConnectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(this.blobContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            string contents = blockBlob.DownloadText();
            return JsonConvert.DeserializeObject<T>(contents);
        }

        public string GetFileAsStringFromBlob(string fileName)
        {
            // Generate string of the file configured in based on file name in Storage configured in AppSettings
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.blobConnectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(this.blobContainerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            return blockBlob.DownloadText();
        }

        public static T GetIntent<T>()
        {
            string contents = GetContents();
            return JsonConvert.DeserializeObject<T>(contents);

        }

        public static string GetContents()
        {
            return File.ReadAllText(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\FormBot.Common\Files\Intentsinfo.json")));
        }
    }

}
