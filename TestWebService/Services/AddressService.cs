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
            var httpClient = _httpClientFactory.CreateClient("GetAddress");
            try
            {
                using HttpResponseMessage httpResponse =
                    await httpClient.PostAsync("https://cleaner.dadata.ru/api/v1/clean/address", requestString);
                return await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
    }
}

