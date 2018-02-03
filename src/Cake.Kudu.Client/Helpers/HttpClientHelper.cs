using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.Kudu.Client.Helpers
{
#pragma warning disable SA1600 // Elements should be documented
    internal static class HttpClientHelper
    {
        internal static TOut HttpPostJsonObject<TIn, TOut>(
            this IKuduClient client,
            string relativeUri,
            TIn value,
            string mediaType = "application/json")
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.PostAsync(
                    requestUri,
                    new StringContent(LitJson.JsonMapper.ToJson(value), Encoding.UTF8, mediaType)),
                JsonResponseToObject<TOut>);
        }

        internal static TOut HttpGetJsonObject<TOut>(
            this IKuduClient client,
            string relativeUri)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.GetAsync(requestUri),
                JsonResponseToObject<TOut>);
        }

        internal static Stream HttpGetStream(
            this IKuduClient client,
            string relativeUri)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.GetAsync(requestUri),
                response =>
                {
                    var ms = new MemoryStream();
                    response.Content.CopyToAsync(ms).GetAwaiter().GetResult();
                    ms.Position = 0;
                    return ms;
            });
        }

        internal static void HttpGetToStream(
            this IKuduClient client,
            string relativeUri,
            Stream stream)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.GetAsync(requestUri),
                response =>
                {
                    response.Content.CopyToAsync(stream).GetAwaiter().GetResult();
                    return true;
                });
        }

        internal static void HttpPutStream(
            this IKuduClient client,
            string relativeUri,
            Stream value)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.PutAsync(requestUri, value == null ? null : new StreamContent(value)),
                response => true);
        }

        internal static void HttpPostStream(
            this IKuduClient client,
            string relativeUri,
            Stream value)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.PostAsync(requestUri, value == null ? null : new StreamContent(value)),
                response => true);
        }

        internal static void HttpPutString(
            this IKuduClient client,
            string relativeUri,
            string value,
            Encoding encoding = null)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.PutAsync(requestUri, new StringContent(value, encoding ?? Encoding.UTF8)),
                response => true);
        }

        internal static void HttpDelete(
            this IKuduClient client,
            string relativeUri)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            ProcessHttpClientAction(
                client,
                relativeUri,
                (httpClient, requestUri) => httpClient.DeleteAsync(requestUri),
                response => true);
        }

        private static TOut JsonResponseToObject<TOut>(HttpResponseMessage response)
        {
            return LitJson.JsonMapper.ToObject<TOut>(response.Content.ReadAsStringAsync().Result);
        }

        private static TOut ProcessHttpClientAction<TOut>(
            this IKuduClient client,
            string relativeUri,
            Func<HttpClient, string, Task<HttpResponseMessage>> preFunc,
            Func<HttpResponseMessage, TOut> postFunc)
        {
            if (string.IsNullOrWhiteSpace(relativeUri))
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            var requestUri = $"{client.Settings.BaseUri.TrimEnd('/')}/{relativeUri.TrimStart('/')}";
            var response = preFunc(client.HttpClient, requestUri).GetAwaiter().GetResult();
            LogAndEnsureSuccessStatusCode(client.Log, response);
            return postFunc(response);
        }

        private static void LogAndEnsureSuccessStatusCode(ICakeLog log, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                log.Verbose("KuduClient: {0} successful: {1:d} ({1:f}).", response.RequestMessage.Method, response.StatusCode);
            }
            else
            {
                log.Error(
                    "KuduClient: {0} failed: {1:d} ({1:f}, {2})",
                    response.RequestMessage.Method,
                    response.StatusCode,
                    response.ReasonPhrase);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    log.Verbose("Url: {0}", response.RequestMessage.RequestUri.AbsoluteUri);
                }
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
