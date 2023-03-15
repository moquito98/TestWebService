using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace TestWebService.Services
{
	public class AddressService: IAddressService
	{
        private readonly IHttpClientFactory _httpClientFactory;
        public AddressService(IHttpClientFactory httpClientFactory)
		{
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<string> GetClearAddressAsync(string requestBody)
        {
            using HttpContent requestString = new StringContent($"[\"{requestBody}\"]", Encoding.UTF8, Application.Json);
            requestString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpClient = _httpClientFactory.CreateClient("GetAddress");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                Console.WriteLine(httpClient.DefaultRequestHeaders.GetValues("Content-Type"));
                using HttpResponseMessage httpResponse =
                await httpClient.PostAsync("https://cleaner.dadata.ru/api/v1/clean/address", requestString);
                //Console.WriteLine(httpResponse);
                return await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
}

