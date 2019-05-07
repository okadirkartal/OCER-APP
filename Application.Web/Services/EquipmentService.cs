using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks; 
using Microsoft.Extensions.Configuration;

namespace Application.Web.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly HttpClient _client = new HttpClient();
        
        public EquipmentService(IConfiguration configuration)
        {
            _client.BaseAddress = new Uri(configuration.GetSection("ApplicationSettings:BaseApiUrl").Value);
            _client.DefaultRequestHeaders.Accept.Clear();

            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            new Dictionary<string, int>();
        }

        public async Task<TResult>  GetAsync<TResult>(string url)
        {
            var response =  await _client.GetAsync(url);

            return await response.Content.ReadAsAsync<TResult>(); 
        }
        
        public async Task<HttpResponseMessage>  GetAsync(string url)
        {
           return await _client.GetAsync(url);
 
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(string url,TModel model)
        {
            return  await _client.PostAsJsonAsync(url,model);
        }

        public void AddHeader(Dictionary<string,int> headerDictionary)
        {
          
            foreach (var pair in headerDictionary)
            {
              _client.DefaultRequestHeaders.Remove(pair.Key);
              _client.DefaultRequestHeaders.Add(pair.Key.ToString(),pair.Value.ToString());  
            }
        }
    }
}