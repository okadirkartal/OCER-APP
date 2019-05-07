using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Web.Services
{
    public interface IEquipmentService
    {
        Task<TResult> GetAsync<TResult>(string url);

        Task<HttpResponseMessage> PostAsJsonAsync<TModel>(string url, TModel model);

        Task<HttpResponseMessage> GetAsync(string url);

        void AddHeader(Dictionary<string,int> headerDictionary);
    }
}