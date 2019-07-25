using AL.library.Utility.Validation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace AL.library.Utility.Web
{
    public class Request : IRequest
    {
        private RestClient restClient;
        private RestRequest restRequest;

        public Request(string urlBase)
        {
            if (urlBase == null)
                throw new InvalidOperationException("URL é obrigatório");

            restClient = new RestClient(urlBase);
            restRequest = new RestRequest();

            //https://stackoverflow.com/questions/12506575/how-to-ignore-the-certificate-check-when-ssl
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //https://docs.telerik.com/fiddler/Configure-Fiddler/Tasks/ConfigureDotNETApp
            //restClient.Proxy = new WebProxy("127.0.0.1", 8888);
        }

        public void addCertificate(X509Certificate2 certificate)
        {
            restClient.ClientCertificates = new X509CertificateCollection();
            restClient.ClientCertificates.Add(certificate);
        }

        public void addArquivo(string name, string path)
        {
            throw new NotImplementedException();
        }

        public void addCookie()
        {
            restClient.CookieContainer = new CookieContainer();
        }

        public void addHeader(Dictionary<string, string> header)
        {
            if (header != null)
                header.ToList().ForEach(x => restRequest.AddHeader(x.Key, x.Value));
        }

        public void addHeader(string key, string value)
        {
            restRequest.AddHeader(key, value);
        }

        public void addSoapBody(object body, string contentType = null)
        {
            if (!string.IsNullOrEmpty(contentType))
                restRequest.AddParameter(contentType, body, ParameterType.RequestBody);
            else
                restRequest.AddParameter("application/soap+xml", body, ParameterType.RequestBody);
        }

        public string get(string urlPath)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.GET;

            IRestResponse response = restClient.Execute(restRequest);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public T get<T>(string urlPath)
        {
            return JsonConvert.DeserializeObject<T>(get(urlPath));
        }

        public async Task<string> getAsync(string urlPath)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.GET;

            var cancellationTokenSource = new CancellationTokenSource();
            IRestResponse response = await restClient.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public async Task<T> getAsync<T>(string urlPath)
        {
            return JsonConvert.DeserializeObject<T>(await getAsync(urlPath));
        }

        public string post(string urlPath, DataFormat dataformat, object body = null)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.POST;
            restRequest.RequestFormat = dataformat;

            if (body != null)
                restRequest.AddBody(body);

            IRestResponse response = restClient.Execute(restRequest);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public T post<T>(string urlPath, DataFormat dataformat, object body = null)
        {
            return JsonConvert.DeserializeObject<T>(post(urlPath, dataformat, body));
        }

        public async Task<string> postAsync(string urlPath, DataFormat dataformat, object body = null)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.POST;
            restRequest.RequestFormat = dataformat;

            if (body != null)
                restRequest.AddBody(body);

            var cancellationTokenSource = new CancellationTokenSource();
            IRestResponse response = await restClient.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public async Task<T> postAsync<T>(string urlPath, DataFormat dataformat, object body = null)
        {
            return JsonConvert.DeserializeObject<T>(await postAsync(urlPath, dataformat, body));
        }

        public string put(string urlPath, DataFormat dataformat, object body = null)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.PUT;
            restRequest.RequestFormat = dataformat;

            if (body != null)
                restRequest.AddBody(body);

            IRestResponse response = restClient.Execute(restRequest);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public T put<T>(string urlPath, DataFormat dataformat, object body = null)
        {
            return JsonConvert.DeserializeObject<T>(put(urlPath, dataformat, body));
        }

        public async Task<string> putAsync(string urlPath, DataFormat dataformat, object body = null)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.PUT;
            restRequest.RequestFormat = dataformat;

            if (body != null)
                restRequest.AddBody(body);

            var cancellationTokenSource = new CancellationTokenSource();
            IRestResponse response = await restClient.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public async Task<T> putAsync<T>(string urlPath, DataFormat dataformat, object body = null)
        {
            return JsonConvert.DeserializeObject<T>(await putAsync(urlPath, dataformat, body));
        }

        public string delete(string urlPath)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.DELETE;

            IRestResponse response = restClient.Execute(restRequest);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public T delete<T>(string urlPath)
        {
            return JsonConvert.DeserializeObject<T>(delete(urlPath));
        }

        public async Task<string> deleteAsync(string urlPath)
        {
            restRequest.Resource = urlPath;
            restRequest.Method = Method.DELETE;

            var cancellationTokenSource = new CancellationTokenSource();
            IRestResponse response = await restClient.ExecuteTaskAsync(restRequest, cancellationTokenSource.Token);

            AssertionConcern.AssertArgumentNotNull(response.Content, "Resposta nulo");
            if (response.ErrorException != null)
                throw new InvalidOperationException(response.ErrorException.Message);

            return response.Content;
        }

        public async Task<T> deleteAsync<T>(string urlPath)
        {
            return JsonConvert.DeserializeObject<T>(await deleteAsync(urlPath));
        }
    }
}
