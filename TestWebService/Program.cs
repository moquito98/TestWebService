using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Dadata;
using Dadata.Model;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using TestWebService.Services;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
builder.Services.AddCors();
builder.Services.AddHttpClient<IAddressService, AddressService>("GetAddress", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://cleaner.dadata.ru/api/v1/clean/address");

    // using Microsoft.Net.Http.Headers;
    // The GitHub API requires two headers.
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Authorization, "Token " + builder.Configuration["token"]);
    httpClient.DefaultRequestHeaders.Add(
        "X-Secret", builder.Configuration["secret"]);
    httpClient.DefaultRequestHeaders.Add(
        "Content-Type", "application/json");
});

var configuration = builder.Configuration;
var app = builder.Build();
IServiceProvider serviceProvider = app.Services;
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
var logger = app.Logger;
app.UseCors(builder => builder.AllowAnyOrigin());

app.UseHttpsRedirection();



app.MapGet("/", async (context) =>
{
    string requestBody = "";
    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
    {
        requestBody = await reader.ReadToEndAsync();
    }
    try
    {
        string responseBody = new AddressService(httpClientFactory).GetClearAddressAsync(requestBody).Result;
        logger.LogInformation(String.Format("[Request]\n{0}\n[Response]\n{1}\n", requestBody, responseBody));
        await context.Response.WriteAsync(responseBody);
    }
    catch(Exception ex)
    {
        logger.LogError(ex.Message);
    }
});

app.Run();

