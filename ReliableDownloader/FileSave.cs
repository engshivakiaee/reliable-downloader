using System;
using System.IO;
using System.Net.Http;

namespace ReliableDownloader
{
    public class FileSave
    {
        public string LocalFilePath { get; set; }
        public HttpResponseMessage Response { get; set; }
        public long RemoteFileSize { get; set; }
        public string TempFilePath { get; set; }
        public FileMode FileMode { get; set; }
        public Action<FileProgress> OnProgressChanged { get; set; }

        public FileSave(string localFilePath,
            HttpResponseMessage httpResponseMessage,
            long remoteFileSize, string tempFilePath,
            FileMode fileMode, Action<FileProgress> onProgressChanged)
        {
            LocalFilePath = localFilePath;
            Response = httpResponseMessage;
            RemoteFileSize = remoteFileSize;
            TempFilePath = tempFilePath;
            FileMode = fileMode;
            OnProgressChanged = onProgressChanged;
        }
    }
}
