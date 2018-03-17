using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NHORS.NStartupInfo;

namespace NHORS.Core
{
    public class HORSBase : IDisposable
    {
        protected readonly HttpClient _httpClient;
        #region Ctor
        public HORSBase(Uri uri,HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public HORSBase(IStartupInfo startupInfo,HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
        #endregion
        #region DataLoader
        public async Task<HttpResponseMessage> Send(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            return await SendAsync(httpRequestMessage, cancellationToken);
        }
        public async Task<HttpResponseMessage> Send(HttpRequestMessage httpRequestMessage)
        {
            return await SendAsync(httpRequestMessage);
        }
        internal protected Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage,CancellationToken cancellationToken)
        {                 
            return _httpClient.SendAsync(httpRequestMessage, cancellationToken);            
        }
        internal protected Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {            
            return _httpClient.SendAsync(httpRequestMessage);
        }
        #endregion
    }
}
