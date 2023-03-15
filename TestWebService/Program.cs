using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Dadata;
using Dadata.Model;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
services.AddHttpClient("foo");
builder.Services.AddCors();

builder.Services.AddHttpClient("GetAddress", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://cleaner.dadata.ru/api/v1/clean/address");

    // using Microsoft.Net.Http.Headers;
    // The GitHub API requires two headers.
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Authorization, "Token " + builder.Configuration["token"]);
    httpClient.DefaultRequestHeaders.Add(
        "X-Secret", builder.Configuration["secret"]);
});

var app = builder.Build();
var logger = app.Logger;
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseHttpsRedirection();

IHttpClientFactory _httpClientFactory;



app.MapGet("/", async (context) =>
{
    string requestBody = "";
    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
    {
        requestBody = await reader.ReadToEndAsync();
    }
    try
    {
        string responseBody = "";
        using (var httpClient = new HttpClient())
        {
            
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://cleaner.dadata.ru/api/v1/clean/address"))
            {
                requestMessage.Headers.Add("Authorization", "Token " + builder.Configuration["token"]);
                requestMessage.Headers.Add("X-Secret", builder.Configuration["secret"]);
                requestMessage.Content = new StringContent(
                    $"[\"{requestBody}\"]",
                    Encoding.UTF8,
                    "application/json");
                HttpResponseMessage result = await httpClient.SendAsync(requestMessage);
                Console.WriteLine(result.StatusCode);
                responseBody = await result.Content.ReadAsStringAsync();
                await context.Response.WriteAsync(responseBody);
            }
        }
        logger.LogInformation(String.Format("[Request]\n{0}\n[Response]\n{1}\n", requestBody, responseBody));
    }
    catch(Exception ex)
    {
        logger.LogError(ex.Message);
    }
});

app.Run();

