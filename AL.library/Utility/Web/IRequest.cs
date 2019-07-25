using RestSharp;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AL.library.Utility.Web
{
    public interface IRequest
    {
        void addHeader(Dictionary<string, string> header);
        void addHeader(string key, string value);
        void addArquivo(string name, string path);
        void addCertificate(X509Certificate2 certificate);
        void addCookie();
        void addSoapBody(object body, string contentType = null);
        string get(string urlPath);
        string post(string urlPath, DataFormat dataformat, object body = null);
        string put(string urlPath, DataFormat dataformat, object body = null);
        string delete(string urlPath);
        T get<T>(string urlPath);
        T post<T>(string urlPath, DataFormat dataformat, object body = null);
        T put<T>(string urlPath, DataFormat dataformat, object body = null);
        T delete<T>(string urlPath);
        Task<string> getAsync(string urlPath);
        Task<string> postAsync(string urlPath, DataFormat dataformat, object body = null);
        Task<string> putAsync(string urlPath, DataFormat dataformat, object body = null);
        Task<string> deleteAsync(string urlPath);
        Task<T> getAsync<T>(string urlPath);
        Task<T> postAsync<T>(string urlPath, DataFormat dataformat, object body = null);
        Task<T> putAsync<T>(string urlPath, DataFormat dataformat, object body = null);
        Task<T> deleteAsync<T>(string urlPath);
    }
}
