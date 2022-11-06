using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        public async Task<bool> DownloadFile(string contentFileUrl, string localFilePath, Action<FileProgress> onProgressChanged)
        {
            long localFileSize = 0;
            var exceptions = new List<Exception>();
            var uniqueExceptions = new List<Exception>();
            var maxTry = 0;

            var responseHeaders = await _webSystemCalls.GetHeadersAsync(contentFileUrl, _cancellationToken.Token);

            if (!responseHeaders.IsSuccessStatusCode)
                throw new Exception($"Http response is not ok. Status code is {responseHeaders.StatusCode} ");

            if (responseHeaders.Content == null)
                throw new Exception("Response content is null");

            var remoteFileSize = responseHeaders.Content.Headers?.ContentRange?.Length
                                ?? responseHeaders.Content?.Headers?.ContentLength
                                ?? 0;
            do
            {
                try
                {
                    var localFileInfo = new FileInfo(localFilePath);

                    if (localFileInfo.Exists)
                    {
                        if (responseHeaders.Headers.AcceptRanges != null
                         && localFileSize > 0 && remoteFileSize > 0
                         && localFileSize != remoteFileSize)
                        {
                            var responsePartiallContent = await _webSystemCalls.DownloadPartialContent(contentFileUrl, localFileSize, remoteFileSize, _cancellationToken.Token);
                            await SaveToFile(new FileSave(localFilePath, responsePartiallContent, remoteFileSize, null, FileMode.Append, onProgressChanged));
                            break;
                        }
                    }

                    var tempFilePath = Path.ChangeExtension(localFilePath, ".tmp");

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    var responseContent = await _webSystemCalls.DownloadContent(contentFileUrl, _cancellationToken.Token);

                    await SaveToFile(new FileSave(localFilePath, responseContent, remoteFileSize, tempFilePath, FileMode.Create, onProgressChanged));
                }

                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                maxTry++;

                // Wait a bit and try again later
                if (exceptions.Any()) await Task.Delay(2000, _cancellationToken.Token);

            }
            while ((remoteFileSize > 0 && localFileSize > 0 && remoteFileSize != localFileSize && !_cancellationToken.IsCancellationRequested)
                || (IsUniqueException(exceptions) && maxTry <= 5));

            if (exceptions.Any())
            {
                throw new AggregateException("Downloading can not be continued", exceptions);
            }

            if (File.Exists(localFilePath))
            {
                return true;
            }

            return false;
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

        private static bool IsNetworkError(Exception ex)
        {
            if (ex is SocketException || ex is WebException)
                return true;
            if (ex.InnerException != null)
                return IsNetworkError(ex.InnerException);
            return false;
        }

        private static bool IsUniqueException(List<Exception> exceptions)
        {
            return exceptions.Distinct().Count() == 1;
        }

        private static TimeSpan CalculateRemainingTime()
        {
            //todo: implement the remaining time
            return new TimeSpan();
        }
    }
}