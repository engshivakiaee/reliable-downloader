using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    public class WebSystemCalls : IWebSystemCalls
    {
        private static HttpClientHandler _handler = new HttpClientHandler
        {
            MaxConnectionsPerServer = int.MaxValue,//to prevent hanging server when many concurrent connections to the server
            UseDefaultCredentials = true
        };

        private static readonly HttpClient _client = new HttpClient(_handler)
        {
            //To prevent facing with TaskCancelledException while speed is low, timout is set to Timeout.InfiniteTimeSpan
            Timeout = Timeout.InfiniteTimeSpan,
        };

        public async Task<HttpResponseMessage> GetHeadersAsync(string url, CancellationToken token)
        {
            return await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url), token).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<HttpResponseMessage> DownloadContent(string url, CancellationToken token)
        {
            //HttpCompletionOption.ResponseHeadersRead tells the Get async method that only receive header completely and then provide the rest of the operation to receive the Response body as a Stream for the continuation of the program
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                return await _client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        public async Task<HttpResponseMessage> DownloadPartialContent(string url, long from, long to, CancellationToken token)
        {
            //HttpCompletionOption.ResponseHeadersRead tells the Get async method that only receive header completely and then provide the rest of the operation to receive the Response body as a Stream for the continuation of the program
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                httpRequestMessage.Headers.Range = new RangeHeaderValue(from, to);
                return await _client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
