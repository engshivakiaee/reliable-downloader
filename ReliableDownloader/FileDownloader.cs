using System;
using System.IO;
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

        private async Task SaveToFile(FileSave fileSave)
        {
            const int maxBufferSize = 0x10000;
            using (var inputStream = await fileSave.Response.Content.ReadAsStreamAsync())
            {
                using (var fileStream = new FileStream(fileSave.FileMode == FileMode.Create ?
                                                        fileSave.TempFilePath :
                                                        fileSave.LocalFilePath,
                                                        fileSave.FileMode,
                                                        FileAccess.Write,
                                                        FileShare.None,
                                                        maxBufferSize,
                                                        useAsync: true
                                                        ))
                {
                    var buffer = new byte[maxBufferSize];
                    int read;
                    var bytesTransferred = 0;
                    var readCount = 0L;
                    while ((read = await inputStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0 && !_cancellationToken.IsCancellationRequested)
                    {
                        bytesTransferred += read;
                        readCount++;

                        if (fileSave.RemoteFileSize > 0 && bytesTransferred > 0)
                        {
                            var percentComplete = Math.Round((decimal)bytesTransferred * 100 / (decimal)fileSave.RemoteFileSize, 2);
                            if (readCount % 100 == 0)
                            {
                                fileSave.OnProgressChanged?.Invoke(new FileProgress(fileSave.RemoteFileSize, bytesTransferred, (double?)percentComplete, CalculateRemainingTime()));
                            }
                        }

                        await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    }
                    fileSave.OnProgressChanged?.Invoke(new FileProgress(fileSave.RemoteFileSize, bytesTransferred, 100, CalculateRemainingTime()));
                }
            }
            if (fileSave.FileMode == FileMode.Create)
                File.Move(fileSave.TempFilePath, fileSave.LocalFilePath);
        }
    }
}