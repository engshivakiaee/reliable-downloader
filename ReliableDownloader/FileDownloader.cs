using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    public class FileDownloader : IFileDownloader
    {
        private readonly IWebSystemCalls _webSystemCalls;
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public FileDownloader(IWebSystemCalls webSystemCalls)
        {
            _webSystemCalls = webSystemCalls;
        }

        public Task<bool> DownloadFile(string contentFileUrl, string localFilePath, Action<FileProgress> onProgressChanged)
        {
            throw new NotImplementedException();
        }

        public void CancelDownloads()
        {
            throw new NotImplementedException();
        }
    }
}